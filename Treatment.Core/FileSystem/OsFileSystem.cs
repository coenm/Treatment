namespace Treatment.Core.FileSystem
{
    using System.IO;

    using Treatment.Core.Interfaces;

    public class OsFileSystem : IFileSystem, IFileSearch
    {
        private OsFileSystem()
        {
        }

        public static OsFileSystem Instance { get; } = new OsFileSystem();

        public string GetFileContent(string filename)
        {
            return File.ReadAllText(filename);
        }

        public void SaveContent(string filename, string content)
        {
            File.WriteAllText(filename, content);
        }

        public string[] FindFilesIncludingSubdirectories(string rootPath, string mask)
        {
            return Directory.GetFiles(rootPath, mask, SearchOption.AllDirectories);
        }
    }
}