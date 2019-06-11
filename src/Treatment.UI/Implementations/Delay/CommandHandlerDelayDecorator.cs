namespace Treatment.UI.Implementations.Delay
{
    using System;
    using System.Diagnostics;
    using System.Reactive.Disposables;
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Core.Configuration;
    using JetBrains.Annotations;
    using Treatment.Contract;
    using Treatment.Helpers.Guards;

    internal class CommandHandlerDelayDecorator<TCommand> : IDisposable, ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        [NotNull] private readonly ICommandHandler<TCommand> decoratee;
        [NotNull] private readonly IDelayService delayService;
        [NotNull] private readonly IReadOnlyConfigurationService configurationService;
        [NotNull] private readonly CompositeDisposable disposables;
        [CanBeNull] private ApplicationSettings configuration;

        public CommandHandlerDelayDecorator(
            [NotNull] ICommandHandler<TCommand> decoratee,
            [NotNull] IReadOnlyConfigurationService configurationService,
            [NotNull] IDelayService delayService)
        {
            Guard.NotNull(decoratee, nameof(decoratee));
            Guard.NotNull(configurationService, nameof(configurationService));
            Guard.NotNull(delayService, nameof(delayService));

            this.delayService = delayService;
            this.configurationService = configurationService;
            this.decoratee = decoratee;

            disposables = new CompositeDisposable(1)
                          {
                              configurationService.ConfigurationChanged.Subscribe(data => configuration = null),
                          };
        }

        [DebuggerStepThrough]
        public async Task ExecuteAsync(TCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default)
        {
            var config = await GetConfigurationAsync();

            if (config != null && config.DelayExecution.Enabled)
                await delayService.DelayAsync(ct).ConfigureAwait(false);

            await decoratee.ExecuteAsync(command, progress, ct).ConfigureAwait(false);
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
