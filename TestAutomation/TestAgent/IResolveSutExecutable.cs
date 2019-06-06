namespace TestAgent
{
    using JetBrains.Annotations;

    public interface IResolveSutExecutable
    {
        [CanBeNull]
        string Executable { get; }
    }
}
