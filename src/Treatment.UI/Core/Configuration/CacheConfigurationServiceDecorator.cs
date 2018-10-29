namespace Treatment.UI.Core.Configuration
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    public class CacheConfigurationServiceDecorator : IConfigurationService
    {
        [NotNull]
        private readonly IConfigurationService decoratee;

        public CacheConfigurationServiceDecorator([NotNull] IConfigurationService decoratee)
        {
            this.decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
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
