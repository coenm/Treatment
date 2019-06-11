namespace Treatment.UI.Implementations.Configuration
{
    using System;
    using System.Threading.Tasks;
    using Core.Core.Configuration;
    using JetBrains.Annotations;
    using Nito.AsyncEx;
    using Treatment.Helpers.Guards;

    [UsedImplicitly]
    public class ConcurrentConfigurationServiceDecorator : IConfigurationService
    {
        [NotNull] private readonly IConfigurationService decoratee;
        [NotNull] private readonly AsyncLock syncLock;

        public ConcurrentConfigurationServiceDecorator([NotNull] IConfigurationService decoratee)
        {
            Guard.NotNull(decoratee, nameof(decoratee));
            this.decoratee = decoratee;

            syncLock = new AsyncLock();
        }

        public IObservable<bool> ConfigurationChanged => decoratee.ConfigurationChanged;

        public async Task<ApplicationSettings> GetAsync()
        {
            using (await syncLock.LockAsync().ConfigureAwait(false))
            {
                return await decoratee.GetAsync().ConfigureAwait(false);
            }
        }

        public async Task<bool> UpdateAsync(ApplicationSettings configuration)
        {
            using (await syncLock.LockAsync().ConfigureAwait(false))
            {
                return await decoratee.UpdateAsync(configuration).ConfigureAwait(false);
            }
        }
    }
}
