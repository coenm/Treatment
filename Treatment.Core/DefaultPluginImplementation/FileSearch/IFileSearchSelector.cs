namespace Treatment.Core.DefaultPluginImplementation.FileSearch
{
    using Treatment.Contract.Plugin.FileSearch;

    public interface IFileSearchSelector
    {
        IFileSearch CreateSearchProvider();
    }
}