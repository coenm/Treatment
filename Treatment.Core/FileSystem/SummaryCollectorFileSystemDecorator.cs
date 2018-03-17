namespace Treatment.Core.FileSystem
{
    using Treatment.Core.Interfaces;

    public class SummaryCollectorFileSystemDecorator : IFileSystem
    {
        private readonly IFileSystem _decoratee;
        private readonly IStatistics _statistics;

        public SummaryCollectorFileSystemDecorator(IFileSystem decoratee, IStatistics statistics)
        {
            _decoratee = decoratee;
            _statistics = statistics;
        }

        public string GetFileContent(string filename)
        {
            _statistics.AddFileRead(filename);
            return _decoratee.GetFileContent(filename);
        }

        public void SaveContent(string filename, string content)
        {
            _statistics.AddFileUpdate(filename);
            _decoratee.SaveContent(filename, content);
        }
    }
}