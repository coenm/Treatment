namespace Treatment.Core.FileSystem
{
    using System.IO;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Treatment.Core.Interfaces;
    using Treatment.Helpers.FileSystem;

    [UsedImplicitly]
    public class SummaryCollectorFileSystemDecorator : IFileSystem
    {
        private readonly IFileSystem decoratee;
        private readonly IStatistics statistics;

        public SummaryCollectorFileSystemDecorator(IFileSystem decoratee, IStatistics statistics)
        {
            this.decoratee = decoratee;
            this.statistics = statistics;
        }

        public bool FileExists(string filename)
        {
            return decoratee.FileExists(filename);
        }

        public Stream OpenRead(string filename, bool useAsync)
        {
            return decoratee.OpenRead(filename, useAsync);
        }

        public Stream OpenWrite(string filename, bool useAsync)
        {
            return decoratee.OpenWrite(filename, useAsync);
        }

        public string GetFileContent(string filename)
        {
            statistics.AddFileRead(filename);
            return decoratee.GetFileContent(filename);
        }

        public void SaveContent(string filename, string content)
        {
            statistics.AddFileUpdate(filename);
            decoratee.SaveContent(filename, content);
        }

        public async Task SaveContentAsync(string filename, Stream content)
        {
            await decoratee.SaveContentAsync(filename, content).ConfigureAwait(false);
        }

        public void DeleteFile(string filename)
        {
            decoratee.DeleteFile(filename);
        }
    }
}
