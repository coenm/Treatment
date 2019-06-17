namespace Treatment.UI.Core.Implementations.Configuration
{
    using JetBrains.Annotations;

    internal interface IConfigFilenameProvider
    {
        [NotNull] string Filename { get; }
    }
}
