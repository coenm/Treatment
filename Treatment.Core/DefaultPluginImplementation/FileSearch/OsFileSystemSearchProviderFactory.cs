namespace Treatment.Core.DefaultPluginImplementation.FileSearch
{
    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.Core.FileSystem;

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
            return OsFileSearch.Instance;
        }
    }
}