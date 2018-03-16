namespace Treatment.Core.Interfaces
{
    public interface ISearchProviderFactory
    {
        int Priority { get; }

        string Name { get; }

        bool CanCreate(string name);

        IFileSearch Create();
    }
}