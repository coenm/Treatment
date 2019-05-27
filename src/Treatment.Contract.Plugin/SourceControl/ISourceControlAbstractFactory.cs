namespace Treatment.Contract.Plugin.SourceControl
{
    using JetBrains.Annotations;

    public interface ISourceControlAbstractFactory
    {
        string Name { get; }

        int Priority { get; }

        IReadOnlySourceControl Create();

        bool CanCreate([NotNull] string name);
    }
}