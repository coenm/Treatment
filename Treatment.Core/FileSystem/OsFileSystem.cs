namespace Treatment.Core.FileSystem
{
    using System.IO;

    using Treatment.Core.Interfaces;

    public class OsFileSystem : IFileSystem
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
    }
}