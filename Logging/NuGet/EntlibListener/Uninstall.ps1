param($installPath, $toolsPath, $package, $project)

$ScriptDirectory = Split-Path $MyInvocation.MyCommand.Path
. (Join-Path $ScriptDirectory NuGetCommon.ps1)