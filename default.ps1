properties {
    $solution_directory = $SolutionPath
    $project_dir_name = $ProjectDirectoryName
    $csproj_filename = if ($CsProjectFileName) { $CsProjectFileName } else { "$ProjectDirectoryName.csproj" }
    
    $project_directory = "$solution_directory\$project_dir_name"
    $csproj_file = "$solution_directory\$project_dir_name\$csproj_filename"
    
    $assembly_version_cs_file = "$solution_directory\$project_dir_name\Properties\VersionAssemblyInfo.cs"
    
    $output_directory = "$solution_directory\build"
    $dist_directory = "$solution_directory\distribution"
    
    $target_config = "Release"
    $framework_versions = "v4.5"
    
    $git_path = "git.exe"
    $svn_path = "svn.exe"
    $nuget_path = "nuget.exe"
    
    $vcs = "git"
    $use_vcs_revision_number = $true
    
    $version = "1.0.0.0"
    $assembly_info_file = "AssemblyInfo"
    $preRelease = $null
	$includeSymbols = $false
}

function Get-RevisionNumber([string]$user_revision_num, [string]$vcs_default) {
    if (-Not $use_vcs_revision_number)
    {
        return $user_revision_num
    }

    $rev = $null
    if ($vcs -eq "git") {
        $rev = Get-GitRevisionNumber
    } elseif ($vcs -eq "svn") {
        $rev = Get-SvnRevisionNumber
    }
    
    if ([System.String]::IsNullOrEmpty($rev))
    {
        $rev = $vcs_default
    }
    else
    {
        $ignore = 0
        if (-Not [System.UInt32]::TryParse($rev, [ref] $ignore))
        {
            $rev = $vcs_default
        }
    }
    
    return $rev
}

function Get-GitRevisionNumber {
    Push-Location $solution_directory
    $oldErrorAction = $ErrorActionPreference
    $ErrorActionPreference = "SilentlyContinue"
    $rev = (. "$git_path" rev-list --count --first-parent HEAD 2> $null)
    $ErrorActionPreference = $oldErrorAction
    Pop-Location
    
    if ([System.String]::IsNullOrEmpty($rev))
    {
        $rev = $null
    }
    
    return $rev
}

function Get-SvnRevisionNumber {
    try
    {
        $xmlStr = (. "$svn_path" info --xml 2> $null)
        $xml = [xml]$xmlStr
        $rev = $xml.info.entry.revision
        
        if ([System.String]::IsNullOrEmpty($rev))
        {
            $rev = $null
        }
        
        return $rev
    }
    catch [system.exception]
    {
        return $null
    }
}

Task default -depends Validate, Clean, CreateNuGetPackage

Task Validate {
    if ([System.String]::IsNullOrEmpty($solution_directory) `
         -Or [System.String]::IsNullOrEmpty($project_dir_name) `
         -Or [System.String]::IsNullOrEmpty($csproj_filename) `
         -Or -Not (Test-Path $solution_directory) `
         -Or -Not (Test-Path $project_directory) `
         -Or -Not (Test-Path $csproj_file))
    {
        throw "Invalid Properties"
    }
}

Task NugetClean {
    if ([System.String]::IsNullOrEmpty($solution_directory) `
         -Or -Not (Test-Path $solution_directory))
    {
        throw "Invalid Properties"
    }
    Remove-Item -Recurse -Force -Path $output_directory -ea SilentlyContinue
    Remove-Item -Recurse -Force -Path $dist_directory -ea SilentlyContinue
}

Task Clean {
    foreach ($singleVersion in $framework_versions.Split(","))
    {
        exec { msbuild /nologo /verbosity:quiet "$csproj_file" "/p:Configuration=$target_config" "/p:TargetFrameworkVersion=$singleVersion" "/p:BaseOutputPath=$output_directory\$project_dir_name" /t:Clean }
    }
}

Task Compile {
    $vSplit = $version.Split('.')
    $ignore = 0
    if($vSplit.Length -ne 4 `
        -Or -Not [System.UInt32]::TryParse($vSplit[0], [ref] $ignore) `
        -Or -Not [System.UInt32]::TryParse($vSplit[1], [ref] $ignore) `
        -Or -Not [System.UInt32]::TryParse($vSplit[2], [ref] $ignore) `
        -Or -Not [System.UInt32]::TryParse($vSplit[3], [ref] $ignore))
    {
        throw "Version number is invalid. Must be in the form of 0.0.0.0"
    }
    $major = $vSplit[0]
    $minor = $vSplit[1]
    $patch = $vSplit[2]
    $rev = Get-RevisionNumber $vSplit[3] "0"
    $packageVersion = "$major.$minor.$patch.$rev"
    
    exec { . "$nuget_path" restore "$solution_directory" -Verbosity quiet }
    foreach ($singleVersion in $framework_versions.Split(","))
    {
        exec { msbuild /nologo /verbosity:quiet "$csproj_file" "/p:Configuration=$target_config" "/p:TargetFrameworkVersion=$singleVersion" "/p:BaseOutputPath=$output_directory\$project_dir_name" "/p:VersionAssembly=$packageVersion" "/p:VersionAssemblyFile=$assembly_info_file" }
    }
}

Task CreateNuGetPackage -depends Compile {
    $vSplit = $version.Split('.')
    $ignore = 0
    if($vSplit.Length -ne 4 `
        -Or -Not [System.UInt32]::TryParse($vSplit[0], [ref] $ignore) `
        -Or -Not [System.UInt32]::TryParse($vSplit[1], [ref] $ignore) `
        -Or -Not [System.UInt32]::TryParse($vSplit[2], [ref] $ignore) `
        -Or -Not [System.UInt32]::TryParse($vSplit[3], [ref] $ignore))
    {
        throw "Version number is invalid. Must be in the form of 0.0.0.0"
    }
    $major = $vSplit[0]
    $minor = $vSplit[1]
    $patch = $vSplit[2]
    $rev = Get-RevisionNumber $vSplit[3] "0"
    if ($preRelease)
    {
        $packageVersion = "$major.$minor.$patch-$preRelease" 
    }
    else
    {
        $packageVersion = "$major.$minor.$patch.$rev"
    }
    
    New-Item $dist_directory -Type Directory -Force | Out-Null
    
    $singleVersion = $framework_versions.Split(",") | Select-Object -Last 1
	if ($includeSymbols) {
		exec { . "$nuget_path" pack "$csproj_file" -Properties "Configuration=$target_config;TargetFrameworkVersion=$singleVersion;BaseOutputPath=$output_directory\$project_dir_name;" -BasePath "$output_directory\$project_dir_name" -OutputDirectory "$dist_directory" -Version "$packageVersion" -Verbosity quiet -Symbols }
	} else {
		exec { . "$nuget_path" pack "$csproj_file" -Properties "Configuration=$target_config;TargetFrameworkVersion=$singleVersion;BaseOutputPath=$output_directory\$project_dir_name;" -BasePath "$output_directory\$project_dir_name" -OutputDirectory "$dist_directory" -Version "$packageVersion" -Verbosity quiet }
	}
}
