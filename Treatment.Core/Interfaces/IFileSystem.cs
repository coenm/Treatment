namespace Treatment.Core.Interfaces
{
    public interface IFileSystem
    {
        string GetFileContent(string filename);

        void SaveContent(string filename, string content);
    }
}