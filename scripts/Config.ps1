$root_dir = ".."

$git_version = $env:gitversion_executable
If ($git_version -eq $null -or ( Test-Path $git_version ) -eq $false )
{
	Write-Host "Set GitVersion to fixed path."
	$git_version = "C:\ProgramData\chocolatey\bin\GitVersion.exe"
}

$git_version_file = "$root_dir\version.json"
	
$output_filename = "$root_dir\src\GitVersionInformation.generated.cs"