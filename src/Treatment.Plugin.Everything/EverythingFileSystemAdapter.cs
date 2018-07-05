namespace Treatment.Plugin.Everything
{
    using JetBrains.Annotations;

    using Treatment.Contract.Plugin.FileSearch;

    public class EverythingFileSeachAdapter : IFileSearch
    {
        public string[] FindFilesIncludingSubdirectories([NotNull]string rootPath, [NotNull]string mask)
        {
            return Everything64Api.Search($"\"{rootPath}\" {mask}").ToArray();
        }
    }
}