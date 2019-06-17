namespace Treatment.UI.Core.Implementations.Configuration
{
    using System;
    using System.Threading.Tasks;
    using Core.Configuration;
    using Helpers.Guards;
    using JetBrains.Annotations;

    [UsedImplicitly]
    public class CacheConfigurationServiceDecorator : IConfigurationService
    {
        [NotNull] private readonly IConfigurationService decoratee;
        [CanBeNull] private ApplicationSettings cachedSettings;

        public CacheConfigurationServiceDecorator([NotNull] IConfigurationService decoratee)
        {
            Guard.NotNull(decoratee, nameof(decoratee));
            this.decoratee = decoratee;
            cachedSettings = null;
        }

        public IObservable<bool> ConfigurationChanged => decoratee.ConfigurationChanged;

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
