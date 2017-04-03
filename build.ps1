$psake_version="4.6.0"
nuget install psake -OutputDirectory .\packages -Version "$psake_version" -Source "nuget.org" -Verbosity quiet

Import-Module .\packages\psake.$psake_version\tools\psake.psm1

$version = "1.1.0.0"
$framework_versions = "netstandard1.2"

$project_name = "Xirsys.Client"
$csproj_filename = "Xirsys.Client.csproj"
"Building {0}" -f $project_name
Invoke-Psake .\default.ps1 "NetStandardPackage" -framework "4.6x64" -parameters @{ "SolutionPath"="$(Resolve-Path .)";"ProjectDirectoryName"="$project_name";"CsProjectFileName"="$csproj_filename"; } -properties @{ "version"="$version";"vcs"="git";"msbuild_path"="C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe";"framework_versions"="$framework_versions";"use_vcs_revision_number"=$true;"includeSymbols"=$true }

Remove-Module psake