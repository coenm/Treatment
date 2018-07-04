namespace Treatment.Contract.Plugin.SourceControl
{
    public interface IReadOnlySourceControl
    {
        FileStatus GetFileStatus(string filename);

        string GetOriginal(string filename);

        string GetModifications(string fileName);
    }
}