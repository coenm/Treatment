namespace Treatment.UI.Implementations.Delay
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Treatment.Contract;
    using Treatment.Helpers.Guards;
    using Treatment.UI.Core.Configuration;

    internal class CommandHandlerDelayDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        [NotNull] private readonly ICommandHandler<TCommand> decoratee;
        [NotNull] private readonly IDelayService delayService;
        [NotNull] private readonly IReadOnlyConfigurationService configurationService;
        private bool? delayEnabled;

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
            delayEnabled = null;
        }

        [DebuggerStepThrough]
        public async Task ExecuteAsync(TCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default(CancellationToken))
        {
            if (delayEnabled.HasValue)
            {
                if (delayEnabled.Value)
                {
                    progress?.Report(new ProgressData("Intentionally delay the execution of the command."));
                    await delayService.DelayAsync(ct).ConfigureAwait(false);
                    progress?.Report(new ProgressData("Delayed the execution enough.."));
                }

                await decoratee.ExecuteAsync(command, progress, ct).ConfigureAwait(false);
                return;
            }

            var configTask = configurationService.GetAsync();
            var commandTask = decoratee.ExecuteAsync(command, progress, ct);
            await Task.WhenAll(configTask, commandTask).ConfigureAwait(false);

            var config = await configTask;
            delayEnabled = config.DelayExecution;

            if (delayEnabled.Value)
            {
                progress?.Report(new ProgressData("Intentionally delay the execution of the command."));
                await delayService.DelayAsync(ct).ConfigureAwait(false);
                progress?.Report(new ProgressData("Delayed the execution enough.."));
            }

            await commandTask;
        }
    }
}
