namespace Treatment.Core.Interfaces
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IFileSystem
    {
        bool FileExists(string filename);

        Stream OpenRead(string filename, bool useAsync);

        Stream OpenWrite(string filename, bool useAsync);

        string GetFileContent(string filename);

        void SaveContent(string filename, string content);

        Task SaveContentAsync(string filename, Stream content);

        void DeleteFile(string filename);
    }
}
