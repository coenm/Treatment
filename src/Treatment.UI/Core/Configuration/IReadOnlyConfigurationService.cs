namespace Treatment.UI.Core.Configuration
{
    using System.Threading.Tasks;

    public interface IReadOnlyConfigurationService
    {
        Task<ApplicationSettings> GetAsync();
    }
}