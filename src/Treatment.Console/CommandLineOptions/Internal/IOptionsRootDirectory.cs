namespace Treatment.Console.CommandLineOptions.Internal
{
    using CommandLine;

    using JetBrains.Annotations;

    internal interface IOptionsRootDirectory
    {
        [Option('d', "directory", Required = false, HelpText = "Root directory to process the csproj files.")]
        string RootDirectory { get; [UsedImplicitly] set; }
    }
}