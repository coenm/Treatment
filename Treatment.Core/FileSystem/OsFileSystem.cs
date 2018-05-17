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

        public bool FileExists(string filename)
        {
            return File.Exists(filename);
        }

        public Stream ReadFile(string filename)
        {
            return File.OpenRead(filename);
        }

        public string GetFileContent(string filename)
        {
            return File.ReadAllText(filename);
        }

        public void SaveContent(string filename, string content)
        {
            File.WriteAllText(filename, content);
        }

        public void SaveContent(string filename, Stream content)
        {
            using (var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                content.CopyTo(fileStream);
            }
        }

        public void DeleteFile(string filename)
        {
            File.Delete(filename);
        }
    }
}