$ScriptName = $MyInvocation.MyCommand.Name

function Generate-Define-Rules ($headerFile, $versionFile)
{
    # Check if the version file exists.
    If (-Not (Test-Path $versionFile)) 
    {
        Write-Host "[!] Version file $versionFile could not be found."
        Exit (3);
    }

    # Read the version file.
    $content = Get-Content ($versionFile)
    $content = $content -join "`n"

    # Create $versionInfo, an object representation of the JSON content.
    
    # Method 1: Does not work on the buildserver (TeamCity) (incompatible powershell version?)
    # $versionInfo = $content | ConvertFrom-Json
    
    # Method 2: Using a .NET assembly.
    [System.Reflection.Assembly]::LoadWithPartialName("System.Web.Extensions") | Out-Null
    $ser = New-Object System.Web.Script.Serialization.JavaScriptSerializer
    $versionInfo = $ser.DeserializeObject($content)

    # Sanity check: was the file correctly parsed, and does it contain everything we need?
    If ($versionInfo -eq $null -or $versionInfo.FullSemVer                -eq $null `
                               -or $versionInfo.InformationalVersion      -eq $null `
                               -or $versionInfo.BranchName                -eq $null `
                               -or $versionInfo.Sha                       -eq $null `
                               -or $versionInfo.Major                     -eq $null `
                               -or $versionInfo.Minor                     -eq $null `
                               -or $versionInfo.Patch                     -eq $null `
                               -or $versionInfo.CommitsSinceVersionSource -eq $null `
                               -or $versionInfo.CommitDate                -eq $null )
    {
        Write-Host "[!] File $versionFile could not be parsed."
        Exit (4);
    }
    # Produce the output.
    Add-Content $headerFile @"
        public const string FullSemanticVersion = "$($versionInfo.FullSemVer)";
        public const string InformationalVersion = "$($versionInfo.InformationalVersion)";
        public const string BranchName = "$($versionInfo.BranchName)";
        public const string Sha = "$($versionInfo.Sha)";
        public const int Major = $($versionInfo.Major);
        public const int Minor = $($versionInfo.Minor);
        public const int Patch = $($versionInfo.Patch);
        public const int CommitsSinceVersionSource = $($versionInfo.CommitsSinceVersionSource);
		public static System.DateTime GitVersionCommitDate = System.DateTime.Parse("$($versionInfo.CommitDate)");
"@
}

# Generate a cs version file.
#
function Generate-Version-File-From-Table ($cs_output_filename, $git_version_file)
{
    # Create the version file, overwriting it if it exists.
    New-Item $cs_output_filename -type file -Force | Out-Null
    
    # Add a header to the file.
    Add-Content $cs_output_filename @"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the ${ScriptName} script 
//     within this repository.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Generated
{
    /// <summary>
    /// Generated version information (using GitVersion)
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode(`"${ScriptName}`", `"1.0.0.0`")]
    [System.Diagnostics.DebuggerStepThrough]
    internal static class GitVersionInfo
    {
"@
       
	# Add content   
	Generate-Define-Rules $cs_output_filename $git_version_file
    	
    # Finally, add a footer.
    Add-Content $cs_output_filename @"
    }
}
"@
}

function Generate-Version-File ($output_filename, $git_version_file)
{
	Write-Host "Generate version file using gitversion."
    & $git_version $root_dir > $git_version_file | Out-Null
    
    # TODO: Check if GitVersion succeeded?
   
    # Generate the version file.
	Write-Host "Generate cs version file."
    Generate-Version-File-From-Table $output_filename $git_version_file
}
