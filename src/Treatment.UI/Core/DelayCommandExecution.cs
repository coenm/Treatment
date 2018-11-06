namespace Treatment.UI.Core
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using SimpleInjector;
    using Treatment.Contract;
    using Treatment.Helpers.Guards;

    /// <summary>
    /// For testing purposes.
    /// </summary>
    public static class DelayCommandExecution
    {
        public static void Register([NotNull] Container container)
        {
            Guard.NotNull(container, nameof(container));

            container.Register(() => new RandomDelayService(2000, 10000), Lifestyle.Singleton);

            container.RegisterDecorator(
                                        typeof(ICommandHandler<>),
                                        typeof(CommandDelayDecorator<>),
                                        Lifestyle.Scoped);
        }

        private class CommandDelayDecorator<TCommand> : ICommandHandler<TCommand>
            where TCommand : ICommand
        {
            private readonly ICommandHandler<TCommand> decoratee;
            private readonly RandomDelayService delayService;

            public CommandDelayDecorator(
                [NotNull] RandomDelayService delayService,
                [NotNull] ICommandHandler<TCommand> decoratee)
            {
                Guard.NotNull(delayService, nameof(delayService));
                Guard.NotNull(decoratee, nameof(decoratee));
                this.delayService = delayService;
                this.decoratee = decoratee;
            }

            [DebuggerStepThrough]
            public async Task ExecuteAsync(TCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default(CancellationToken))
            {
                progress?.Report(new ProgressData("Intentionally delay the execution of the command."));
                await delayService.DelayAsync(ct).ConfigureAwait(false);
                progress?.Report(new ProgressData("Delayed the execution enough.."));

                await decoratee.ExecuteAsync(command, progress, ct).ConfigureAwait(false);
            }
        }

        private class RandomDelayService
        {
            private readonly int minMilliseconds;
            private readonly int maxMilliseconds;
            [NotNull] private readonly Random random;

            public RandomDelayService(int minMilliseconds, int maxMilliseconds)
            {
                this.minMilliseconds = minMilliseconds;
                this.maxMilliseconds = maxMilliseconds;
                random = new Random();
            }

            public async Task DelayAsync(CancellationToken ct = default(CancellationToken))
            {
                var millisecondsDelay = random.Next(minMilliseconds, maxMilliseconds);
                await Task.Delay(millisecondsDelay, ct).ConfigureAwait(false);
            }
        }
    }
}
