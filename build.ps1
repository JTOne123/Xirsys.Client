$psake_version="4.4.2"
nuget install psake -OutputDirectory .\packages -Version "$psake_version" -Source "nuget.org" -Verbosity quiet

Import-Module .\packages\psake.$psake_version\tools\psake.psm1

$version = "1.0.1.0"
$framework_versions = "v4.5"

Invoke-Psake .\default.ps1 "NugetClean" -framework "4.6x64" -parameters @{ "SolutionPath"="$(Resolve-Path .)"; }

$project_name = "src\Xirsys.Client"
$csproj_filename = "Xirsys.Client.csproj"
$xproj_filename = "Xirsys.Client.xproj"
"Building {0}" -f $project_name
Invoke-Psake .\default.ps1 "DotNetPack" -framework "4.6x64" -parameters @{ "SolutionPath"="$(Resolve-Path .)";"ProjectDirectoryName"="$project_name";"CsProjectFileName"="$csproj_filename";"XProjectFileName"="$xproj_filename"; } -properties @{ "version"="$version";"vcs"="git";"framework_versions"="$framework_versions";"use_vcs_revision_number"=$true;"includeSymbols"=$true }

Remove-Module psake