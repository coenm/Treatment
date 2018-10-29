using JetBrains.Annotations;

namespace Treatment.UI.Core.Configuration
{
    public interface IConfiguration
    {
        [CanBeNull]
        string RootPath { get; }
    }
}
