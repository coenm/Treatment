namespace Treatment.Core.Interfaces
{
    using System.IO;

    public interface IFileSystem
    {
        bool FileExists(string filename);

        Stream ReadFile(string filename);

        string GetFileContent(string filename);

        void SaveContent(string filename, string content);

        void SaveContent(string filename, Stream content);

        void DeleteFile(string filename);
    }
}