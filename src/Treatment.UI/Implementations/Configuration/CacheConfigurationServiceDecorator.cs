namespace Treatment.UI.Implementations.Configuration
{
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Treatment.Helpers;
    using Treatment.UI.Core.Configuration;

    public class CacheConfigurationServiceDecorator : IConfigurationService
    {
        [NotNull] private readonly IConfigurationService decoratee;
        [CanBeNull] private ApplicationSettings cachedSettings;

        public CacheConfigurationServiceDecorator([NotNull] IConfigurationService decoratee)
        {
            this.decoratee = Guard.NotNull(decoratee, nameof(decoratee));
            cachedSettings = null;
        }

        public async Task<ApplicationSettings> GetAsync()
        {
            if (cachedSettings == null)
                cachedSettings = await decoratee.GetAsync().ConfigureAwait(false);

            return cachedSettings;
        }

        public async Task<bool> UpdateAsync(ApplicationSettings configuration)
        {
            if (!await decoratee.UpdateAsync(configuration).ConfigureAwait(false))
                return false;

            cachedSettings = configuration;
            return true;
        }
    }
}
