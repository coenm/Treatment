namespace Treatment.Core.DefaultPluginImplementation.FileSearch
{
    using System.IO;

    using Treatment.Contract.Plugin.FileSearch;

    public class OsFileSearch : IFileSearch
    {
        private OsFileSearch()
        {
        }

        public static OsFileSearch Instance { get; } = new OsFileSearch();

        public string[] FindFilesIncludingSubdirectories(string rootPath, string mask)
        {
            return Directory.GetFiles(rootPath, mask, SearchOption.AllDirectories);
        }
    }
}