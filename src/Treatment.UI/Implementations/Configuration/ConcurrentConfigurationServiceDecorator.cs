namespace Treatment.UI.Implementations.Configuration
{
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Nito.AsyncEx;
    using Treatment.Helpers;
    using Treatment.UI.Core.Configuration;

    public class ConcurrentConfigurationServiceDecorator : IConfigurationService
    {
        [NotNull] private readonly IConfigurationService decoratee;
        [NotNull] private readonly AsyncLock syncLock;

        public ConcurrentConfigurationServiceDecorator([NotNull] IConfigurationService decoratee)
        {
            this.decoratee = Guard.NotNull(decoratee, nameof(decoratee));
            syncLock = new AsyncLock();
        }

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
