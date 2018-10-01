namespace Treatment.Plugin.Everything
{
    using System.Linq;

    using JetBrains.Annotations;

    using Treatment.Contract.Plugin.FileSearch;

    public class EverythingFileSearchAdapter : IFileSearch
    {
        public string[] FindFilesIncludingSubdirectories([NotNull]string rootPath, [NotNull]string mask)
        {
            return Everything64Api.Search($"\"{rootPath}\" {mask}").ToArray();
        }
    }
}