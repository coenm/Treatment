namespace Treatment.Contract.Plugin.FileSearch
{
    public interface IFileSearch
    {
        string[] FindFilesIncludingSubdirectories(string rootPath, string mask);
    }
}