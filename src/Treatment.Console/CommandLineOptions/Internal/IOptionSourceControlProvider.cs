namespace Treatment.Console.CommandLineOptions.Internal
{
    using CommandLine;

    using JetBrains.Annotations;

    internal interface IOptionSourceControlProvider
    {
        [Option("versioncontrol-provider", Required = false, HelpText = "Set version control provider")]
        string SourceControlProvider { get; [UsedImplicitly] set; }
    }
}