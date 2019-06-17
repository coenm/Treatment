namespace TestAgent.Model.Configuration
{
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    public interface IConfigurationService : IReadOnlyConfigurationService
    {
        Task<bool> UpdateAsync([NotNull] TestAgentApplicationSettings configuration);
    }
}
