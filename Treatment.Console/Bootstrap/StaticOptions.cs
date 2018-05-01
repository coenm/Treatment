namespace Treatment.Console.Bootstrap
{
    using Treatment.Core.DefaultPluginImplementation.FileSearch;

    public class StaticOptions : IVerboseOption, IDryRunOption, IHoldOnExitOption, ISearchProviderNameOption
    {
        public StaticOptions(
            VerboseLevel level,
            bool isDryRun,
            bool holdOnExit,
            string searchProviderName)
        {
            IsDryRun = isDryRun;
            HoldOnExit = holdOnExit;
            SearchProviderName = searchProviderName;
            Level = level;
        }
        public VerboseLevel Level { get; }

        public bool IsDryRun { get; }

        public bool HoldOnExit { get; }

        public string SearchProviderName { get; }
    }
}