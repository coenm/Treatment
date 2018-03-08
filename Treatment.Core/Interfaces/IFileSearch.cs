namespace Treatment.Core.Interfaces
{
    public interface IFileSearch
    {
        string[] FindFilesIncludingSubdirectories(string rootPath, string mask);
    }
}