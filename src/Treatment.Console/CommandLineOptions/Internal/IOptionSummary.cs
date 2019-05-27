namespace Treatment.Console.CommandLineOptions.Internal
{
    using CommandLine;

    using JetBrains.Annotations;

    internal interface IOptionSummary
    {
        [Option('s', "summary", Default = false, Required = false, HelpText = "Prints a summary at the end of fixing the csproj files.")]
        bool Summary { get; [UsedImplicitly] set; }
    }
}