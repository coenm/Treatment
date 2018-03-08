namespace Treatment.Everything
{
    using JetBrains.Annotations;

    using Treatment.Core.Interfaces;

    public class EverythingFileSeachAdapter : IFileSearch
    {
        public string[] FindFilesIncludingSubdirectories([NotNull]string rootPath, [NotNull]string mask)
        {
            return Everything32Api.Search($"\"{rootPath}\" {mask}").ToArray();
        }
    }
}