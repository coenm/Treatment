namespace Treatment.UI.Implementations.Delay
{
    using System;
    using System.Reactive.Disposables;
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Core.Configuration;
    using JetBrains.Annotations;

    using Treatment.Helpers.Guards;

    internal class RandomConfigurableDelayService : IDisposable, IDelayService
    {
        [NotNull] private readonly Random random;
        [NotNull] private readonly IReadOnlyConfigurationService configurationService;
        [NotNull] private readonly CompositeDisposable disposables;
        [CanBeNull] private ApplicationSettings configuration;

        public RandomConfigurableDelayService([NotNull] IReadOnlyConfigurationService configurationService)
        {
            Guard.NotNull(configurationService, nameof(configurationService));
            this.configurationService = configurationService;
            disposables = new CompositeDisposable(1);
            random = new Random();

            var subscription = configurationService.ConfigurationChanged.Subscribe(data => configuration = null);
            disposables.Add(subscription);
        }

        public async Task DelayAsync(CancellationToken ct = default)
        {
            var config = await GetConfigurationAsync();

            if (config == null || config.DelayExecution.Enabled == false)
                return;

            var millisecondsDelay = random.Next(config.DelayExecution.MinMilliseconds, config.DelayExecution.MaxMilliseconds);

            if (millisecondsDelay == 0)
                return;

            await Task.Delay(millisecondsDelay, ct).ConfigureAwait(false);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }

        private async Task<ApplicationSettings> GetConfigurationAsync()
        {
            var config = configuration;
            if (config != null)
                return config;

            configuration = await configurationService.GetAsync().ConfigureAwait(false);
            return configuration;
        }
    }
}
