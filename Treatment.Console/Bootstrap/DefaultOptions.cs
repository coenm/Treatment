namespace Treatment.Console.Bootstrap
{
    using Treatment.Core.DefaultPluginImplementation.FileSearch;

    public class DefaultOptions : IVerboseOption, IDryRunOption, IHoldOnExitOption, ISearchProviderNameOption
    {
        public VerboseLevel Level => VerboseLevel.Null;

        public bool IsDryRun => false;

        public bool HoldOnExit => false;

        public string SearchProviderName => string.Empty;
    }
}