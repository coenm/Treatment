namespace Treatment.Console.Options
{
    using CommandLine;

    using JetBrains.Annotations;

    [Verb("fix", HelpText = "Fix csproj files where hintpath of packages is fixed")]
    public class FixOptions : Options
    {
        [Option('d', "directory", Required = true, HelpText = "Root directory to process the csproj files.")]
        public string RootDirectory { get; [UsedImplicitly] set; }

        [Option('n', "dry-run", Default = false, Required = false, HelpText = "File changes are not written to disk, only listed in the console.")]
        public bool DryRun { get; [UsedImplicitly] set; }

        [Option('s', "summary", Default = false, Required = false, HelpText = "Prints a summary at the end of fixing the csproj files.")]
        public bool Summary { get; [UsedImplicitly] set; }

        [Option('p', "search-provider", Required = false, Default = "FileSystem", HelpText = "Set search provider to search for csproj files. To list the search providers, use the 'list-providers' command.")]
        public string SearchProvider { get; [UsedImplicitly] set; }

        [Option('v', "verbose", Default = false, Required = false, HelpText = "Prints more information about the current process to the console.")]
        public bool Verbose { get; [UsedImplicitly] set; }
    }
}