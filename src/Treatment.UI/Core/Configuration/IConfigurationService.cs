namespace Treatment.UI.Core.Configuration
{
    using System.Threading.Tasks;

    public interface IConfigurationService
    {
        Task<ApplicationSettings> GetAsync();

        Task<bool> UpdateAsync(ApplicationSettings configuration);

        // temp
        IConfiguration GetConfiguration();
    }
}
