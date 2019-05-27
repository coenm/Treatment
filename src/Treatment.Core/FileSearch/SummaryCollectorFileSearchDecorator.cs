namespace Treatment.Core.FileSearch
{
    using JetBrains.Annotations;

    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.Core.Interfaces;

    [UsedImplicitly]
    public class SummaryCollectorFileSearchDecorator : IFileSearch
    {
        private readonly IFileSearch decoratee;
        private readonly IStatistics statistics;

        public SummaryCollectorFileSearchDecorator(IFileSearch decoratee, IStatistics statistics)
        {
            this.decoratee = decoratee;
            this.statistics = statistics;
        }

        public string[] FindFilesIncludingSubdirectories(string rootPath, string mask)
        {
            var result = decoratee.FindFilesIncludingSubdirectories(rootPath, mask);
            statistics.AddFoundFiles(result);
            return result;
        }
    }
}
