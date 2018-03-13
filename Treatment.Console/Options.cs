namespace Treatment.Console
{
    using CommandLine;

    using JetBrains.Annotations;

    public enum SearchProvider
    {
        FileSystem,
        Everything
    }

    public class Options
    {
        [Option('n', "dry-run", Default = false, Required = false, HelpText = "Dry run")]
        public bool DryRun { get; [UsedImplicitly] set; }

        [Option('v', "verbose", Default = false, Required = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; [UsedImplicitly] set; }

        [Option('s', "summary", Default = true, Required = false, HelpText = "Summary")]
        public bool Summary { get; [UsedImplicitly] set; }

        [Option('h', "hold", Default = false, Required = false, HelpText = "Keeps console open when process successfully finished.")]
        public bool HoldOnExit { get; [UsedImplicitly] set; }

        [Option('p', "search-provider", Required = false, Default = SearchProvider.FileSystem, HelpText = "Set search provider to search for csproj files. Options: FileSystem, Everything")]
        public SearchProvider SearchProvider { get; [UsedImplicitly] set; }

        [Option('d', "directory", Required = true, HelpText = "Root directory to process the csproj files.")]
        public string RootDirectory { get; [UsedImplicitly] set; }
    }
}