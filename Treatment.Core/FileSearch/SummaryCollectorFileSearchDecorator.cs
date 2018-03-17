namespace Treatment.Core.FileSearch
{
    using Treatment.Core.Interfaces;

    public class SummaryCollectorFileSearchDecorator : IFileSearch
    {
        private readonly IFileSearch _decoratee;
        private readonly IStatistics _statistics;

        public SummaryCollectorFileSearchDecorator(IFileSearch decoratee, IStatistics statistics)
        {
            _decoratee = decoratee;
            _statistics = statistics;
        }

        public string[] FindFilesIncludingSubdirectories(string rootPath, string mask)
        {
            var result = _decoratee.FindFilesIncludingSubdirectories(rootPath, mask);
            _statistics.AddFoundFiles(result);
            return result;
        }
    }
}