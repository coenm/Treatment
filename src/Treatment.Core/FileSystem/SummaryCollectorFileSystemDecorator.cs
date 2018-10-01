namespace Treatment.Core.FileSystem
{
    using System.IO;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Treatment.Core.Interfaces;

    [UsedImplicitly]
    public class SummaryCollectorFileSystemDecorator : IFileSystem
    {
        private readonly IFileSystem _decoratee;
        private readonly IStatistics _statistics;

        public SummaryCollectorFileSystemDecorator(IFileSystem decoratee, IStatistics statistics)
        {
            _decoratee = decoratee;
            _statistics = statistics;
        }

        public bool FileExists(string filename)
        {
            return _decoratee.FileExists(filename);
        }

        public Stream ReadFile(string filename)
        {
            return _decoratee.ReadFile(filename);
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

        public async Task SaveContentAsync(string filename, Stream content)
        {
            await _decoratee.SaveContentAsync(filename, content).ConfigureAwait(false);
        }

        public void DeleteFile(string filename)
        {
            _decoratee.DeleteFile(filename);
        }
    }
}