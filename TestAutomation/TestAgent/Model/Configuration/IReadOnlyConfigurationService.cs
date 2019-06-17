namespace TestAgent.Model.Configuration
{
    using System.Threading.Tasks;

    public interface IReadOnlyConfigurationService
    {
        Task<TestAgentApplicationSettings> GetAsync();
    }
}
