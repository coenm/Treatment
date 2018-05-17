namespace Treatment.Console.Bootstrap
{
    using Treatment.Core.DefaultPluginImplementation.FileSearch;
    using Treatment.Core.DefaultPluginImplementation.SourceControl;

    public class StaticOptions : IVerboseOption, IDryRunOption, IHoldOnExitOption, ISearchProviderNameOption, ISourceControlNameOption
    {
        public StaticOptions(
            VerboseLevel level,
            bool isDryRun,
            bool holdOnExit,
            string searchProviderName,
            string sourceControlProviderName)
        {
            IsDryRun = isDryRun;
            HoldOnExit = holdOnExit;
            SearchProviderName = searchProviderName;
            SourceControlProviderName = sourceControlProviderName;
            Level = level;
        }
        public VerboseLevel Level { get; }

        public bool IsDryRun { get; }

        public bool HoldOnExit { get; }

        public string SearchProviderName { get; }

        public string SourceControlProviderName { get; }
    }
}