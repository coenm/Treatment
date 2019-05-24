namespace Treatment.Console.CommandLineOptions
{
    using CommandLine;

    using JetBrains.Annotations;

    using Treatment.Console.CommandLineOptions.Internal;

    [Verb("full-version", HelpText = "Display full version information.")]
    [UsedImplicitly]
    public class DisplayVersionOption : OptionsBase, IOptionsHoldOnExit
    {
        public bool HoldOnExit { get; set; }
    }
}
