# Build script that is called when building the software in visual studio.
# More specifically, this is called before starting the build.

# Set the script folder as the current working directory.
$scriptpath = $MyInvocation.MyCommand.Path

# Save the current location.
$CurrentDir = $(Get-Location).Path;
Write-Host "CurrentDir: " $CurrentDir

# Get location of powershell file
Write-Host "PSScriptRoot: " $PSScriptRoot

# we know this script is located in the .scripts\ folder of the root.
$RootDir = [IO.Path]::GetFullPath( (join-path $PSScriptRoot "..\") )
Write-Host "ROOT: " $RootDir


Write-Host "scriptpath: $scriptpath"
Set-Location -Path (Split-Path $scriptpath) 





# Include the relevant modules.
. .\Config.ps1
. .\GenerateVersionFile.ps1

Generate-Version-File $output_filename $git_version_file

$cd = $(Get-Location).Path;
Write-Host "current dir: $cd"