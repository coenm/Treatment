namespace Treatment.Console
{
    using CommandLine;

    using JetBrains.Annotations;

    public class Options
    {
        [Option('n', "dry-run", Default = false, Required = false, HelpText = "Dry run")]
        public bool DryRun { get; [UsedImplicitly] set; }

        [Option('v', "verbose", Default = false, Required = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; [UsedImplicitly] set; }

        [Option('s', "summary", Default = false, Required = false, HelpText = "Summary")]
        public bool Summary { get; [UsedImplicitly] set; }

        [Option('h', "hold", Default = false, Required = false, HelpText = "Keeps console open when process successfully finished.")]
        public bool HoldOnExit { get; [UsedImplicitly] set; }

        [Option('p', "search-provider", Required = false, Default = "FileSystem", HelpText = "Set search provider to search for csproj files. Default: FileSystem. To get the list of providers. Use the -l option.")]
        public string SearchProvider { get; [UsedImplicitly] set; }

        [Option('l', "list-search-providers", Required = false, Default = false, HelpText = "List search providers.")]
        public bool ListProviders { get; [UsedImplicitly] set; }

        [Option('d', "directory", Required = true, HelpText = "Root directory to process the csproj files.")]
        public string RootDirectory { get; [UsedImplicitly] set; }
    }
}