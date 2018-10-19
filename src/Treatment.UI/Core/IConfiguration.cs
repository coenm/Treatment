namespace Treatment.UI.Core
{
    using JetBrains.Annotations;

    public interface IConfiguration
    {
        [CanBeNull]
        string RootPath { get; }
    }
}