namespace Treatment.Console.CommandLineOptions.Internal
{
    using CommandLine;

    using JetBrains.Annotations;

    internal interface IOptionSearchProvider
    {
        [Option('p', "search-provider", Required = false, Default = "FileSystem", HelpText = "Set search provider to search for csproj files. To list the search providers, use the 'list-providers' command.")]
        string SearchProvider { get; [UsedImplicitly] set; }
    }
}