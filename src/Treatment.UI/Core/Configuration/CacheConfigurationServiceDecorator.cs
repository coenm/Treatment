namespace Treatment.UI.Core.Configuration
{
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Treatment.Helpers;

    public class CacheConfigurationServiceDecorator : IConfigurationService
    {
        [NotNull]
        private readonly IConfigurationService decoratee;

        public CacheConfigurationServiceDecorator([NotNull] IConfigurationService decoratee)
        {
            this.decoratee = Guard.NotNull(decoratee, nameof(decoratee));
        }

        public Task<ApplicationSettings> GetAsync()
        {
            return decoratee.GetAsync();
        }

        public Task<bool> UpdateAsync(ApplicationSettings configuration)
        {
            return decoratee.UpdateAsync(configuration);
        }

        public IConfiguration GetConfiguration() => decoratee.GetConfiguration();
    }
}
