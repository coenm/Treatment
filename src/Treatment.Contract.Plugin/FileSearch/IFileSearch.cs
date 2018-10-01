namespace Treatment.Contract.Plugin.FileSearch
{
    using JetBrains.Annotations;

    public interface IFileSearch
    {
        string[] FindFilesIncludingSubdirectories([NotNull] string rootPath, [NotNull] string mask);
    }
}