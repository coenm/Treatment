namespace TestAgent.Model
{
    using JetBrains.Annotations;

    internal interface IConfigFilenameProvider
    {
        [NotNull] string Filename { get; }
    }
}
