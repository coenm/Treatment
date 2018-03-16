namespace Treatment.Core.FileSystem
{
    using Treatment.Core.Interfaces;

    public class OsFileSystemSearchProviderFactory : ISearchProviderFactory
    {
        public int Priority { get; } = int.MaxValue;

        public string Name { get; } = "FileSystem";

        public bool CanCreate(string name)
        {
            return true;
        }

        public IFileSearch Create()
        {
            return OsFileSystem.Instance;
        }
    }
}