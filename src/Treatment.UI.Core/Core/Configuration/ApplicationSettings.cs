namespace Treatment.UI.Core.Core.Configuration
{
    using JetBrains.Annotations;

    public class ApplicationSettings
    {
        [NotNull]
        public DelayExecutionSettings DelayExecution { get; set; }

        public string SearchProviderName { get; set; }

        public string VersionControlProviderName { get; set; }

        public string RootDirectory { get; set; }
    }
}
