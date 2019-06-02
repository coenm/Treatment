namespace TestAgent
{
    using JetBrains.Annotations;

    public interface ISutExecutable
    {
        [CanBeNull]
        string Executable { get; }
    }
}
