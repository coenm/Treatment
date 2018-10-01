namespace Treatment.Console.CommandLineOptions
{
    using CommandLine;

    using JetBrains.Annotations;

    using Treatment.Console.CommandLineOptions.Internal;

    [Verb("fix", HelpText = "Fix csproj files where hint path of packages is fixed")]

    public class FixOptions : OptionsBase,
                              IOptionsHoldOnExit,
                              IOptionsRootDirectory,
                              IOptionsDryRun,
                              IOptionsVerbose,
                              IOptionSearchProvider,
                              IOptionSummary
    {
        public string RootDirectory { get; [UsedImplicitly] set; }

        public bool DryRun { get; [UsedImplicitly] set; }

        public bool Summary { get; [UsedImplicitly] set; }

        public string SearchProvider { get; [UsedImplicitly] set; }

        public int Verbose { get; [UsedImplicitly] set; }

        public bool HoldOnExit { get; [UsedImplicitly] set; }
    }
}