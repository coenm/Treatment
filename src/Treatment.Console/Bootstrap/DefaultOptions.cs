namespace Treatment.Console.Bootstrap
{
    using Treatment.Core.DefaultPluginImplementation.FileSearch;
    using Treatment.Core.DefaultPluginImplementation.SourceControl;

    public class DefaultOptions : IVerboseOption, IDryRunOption, IHoldOnExitOption, ISearchProviderNameOption, ISourceControlNameOption
    {
        public VerboseLevel Level => VerboseLevel.Disabled;

        public bool IsDryRun => false;

        public bool HoldOnExit => false;

        public string SearchProviderName => string.Empty;

        public string SourceControlProviderName => string.Empty;
    }
}