namespace Treatment.Console.CommandLineOptions
{
    using CommandLine;

    using JetBrains.Annotations;

    [Verb("list-providers", HelpText = "List installed search providers to be used when fixing csproject files.")]
    [UsedImplicitly]
    public class ListProvidersOptions : Options
    {
    }
}