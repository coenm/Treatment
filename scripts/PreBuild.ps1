# Build script that is called when building the software in visual studio.
# More specifically, this is called before starting the build.

# Set the script folder as the current working directory.
$scriptpath = $MyInvocation.MyCommand.Path

# Save the current location.
$CurrentDir = $(Get-Location).Path;


Try
{
	Set-Location -Path (Split-Path $scriptpath) 

	# Include the relevant modules.
	. .\Config.ps1
	. .\GenerateVersionFile.ps1
	Generate-Version-File $output_filename $git_version_file
}
Finally
{
	cd $CurrentDir
}