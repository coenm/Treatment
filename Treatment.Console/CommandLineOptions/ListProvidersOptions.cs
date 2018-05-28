namespace Treatment.Console.CommandLineOptions
{
    using CommandLine;

    using JetBrains.Annotations;

    using Treatment.Console.CommandLineOptions.Internal;

    [Verb("list-providers", HelpText = "List installed search, and version control providers to be used")]
    [UsedImplicitly]
    public class ListProvidersOptions : OptionsBase,
                                        IOptionsHoldOnExit
    {
        [Option('h', "hold", Default = false, Required = false, HelpText = "Keeps console open when process successfully finished.")]
        public bool HoldOnExit { get; [UsedImplicitly] set; }
    }
}