# we know this script is located in the .scripts\ folder of the root.
$root_dir = [IO.Path]::GetFullPath( (join-path $PSScriptRoot "..\") )
Write-Host "Root directory : " $root_dir


$git_version = $env:gitversion_executable
If ($git_version -eq $null -or ( Test-Path $git_version ) -eq $false )
{
	$git_version = "C:\ProgramData\chocolatey\bin\GitVersion.exe"
	Write-Host "Set GitVersion to fixed path: $git_version"
}

$git_version_file = [IO.Path]::GetFullPath( (join-path $root_dir "version.json") )
$output_filename = [IO.Path]::GetFullPath( (join-path $root_dir "src\GitVersionInformation.generated.cs") )
