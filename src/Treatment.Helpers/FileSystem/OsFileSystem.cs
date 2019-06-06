namespace Treatment.Helpers.FileSystem
{
    using System.IO;
    using System.Threading.Tasks;

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

        public Stream OpenRead(string filename, bool useAsync)
        {
            if (useAsync)
                return new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize: 4096, useAsync: true);

            return File.OpenRead(filename);
        }

        public Stream OpenWrite(string filename, bool useAsync)
        {
            if (useAsync)
                return new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);

            return File.OpenWrite(filename);
        }

        public string GetFileContent(string filename)
        {
            return File.ReadAllText(filename);
        }

        public void SaveContent(string filename, string content)
        {
            File.WriteAllText(filename, content);
        }

        public async Task SaveContentAsync(string filename, Stream content)
        {
            using (var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                await content.CopyToAsync(fileStream).ConfigureAwait(false);
            }
        }

        public void DeleteFile(string filename)
        {
            File.Delete(filename);
        }
    }
}
