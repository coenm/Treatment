namespace Treatment.Contract.Plugin.SourceControl
{
    using JetBrains.Annotations;

    public interface IReadOnlySourceControl
    {
        FileStatus GetFileStatus([NotNull] string filename);

        string GetOriginal([NotNull] string filename);

        string GetModifications([NotNull] string fileName);
    }
}