namespace Treatment.Contract.Plugin.SourceControl
{
    public interface ISourceControlAbstractFactory
    {
        string Name { get; }

        int Priority { get; }

        IReadOnlySourceControl Create();

        bool CanCreate(string name);
    }
}