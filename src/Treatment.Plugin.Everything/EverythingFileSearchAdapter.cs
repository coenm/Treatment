namespace Treatment.Plugin.Everything
{
    using System.Linq;

    using Treatment.Contract.Plugin.FileSearch;

    internal class EverythingFileSearchAdapter : IFileSearch
    {
        public string[] FindFilesIncludingSubdirectories(string rootPath, string mask)
        {
            return Everything64Api.Search($"\"{rootPath}\" {mask}").ToArray();
        }
    }
}