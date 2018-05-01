namespace Treatment.Contract.Plugin.FileSearch
{
    public interface ISearchProviderFactory
    {
        int Priority { get; }

        string Name { get; }

        bool CanCreate(string name);

        IFileSearch Create();
    }
}