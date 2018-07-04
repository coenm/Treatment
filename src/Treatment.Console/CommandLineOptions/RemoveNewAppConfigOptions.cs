namespace Treatment.Console.CommandLineOptions
{
    using CommandLine;

    using JetBrains.Annotations;

    using Treatment.Console.CommandLineOptions.Internal;

    [Verb("fix-app-config", HelpText = "Remove new app.config files and fix csproj file")]
    public class RemoveNewAppConfigOptions : OptionsBase,
                                             IOptionsHoldOnExit,
                                             IOptionsRootDirectory,
                                             IOptionsDryRun,
                                             IOptionsVerbose,
                                             IOptionSearchProvider,
                                             IOptionSourceControlProvider,
                                             IOptionSummary
    {
        public string RootDirectory { get; [UsedImplicitly] set; }

        public bool DryRun { get; [UsedImplicitly] set; }

        public bool Summary { get; [UsedImplicitly] set; }

        public string SearchProvider { get; [UsedImplicitly] set; }

        public int Verbose { get; [UsedImplicitly] set; }

        public bool HoldOnExit { get; [UsedImplicitly] set; }

        public string SourceControlProvider { get; [UsedImplicitly] set; }
    }
}