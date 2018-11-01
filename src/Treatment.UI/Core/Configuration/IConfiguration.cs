namespace Treatment.UI.Core.Configuration
{
    using JetBrains.Annotations;

    public interface IConfiguration
    {
        [CanBeNull] string RootPath { get; }
    }
}
