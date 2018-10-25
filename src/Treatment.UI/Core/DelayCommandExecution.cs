namespace Treatment.UI.Core
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using SimpleInjector;

    using Treatment.Contract;

    /// <summary>
    /// For testing purposes.
    /// </summary>
    public static class DelayCommandExecution
    {
        public static void Register([NotNull] Container container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

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

            public CommandDelayDecorator([NotNull] RandomDelayService delayService, [NotNull] ICommandHandler<TCommand> decoratee)
            {
                this.delayService = delayService ?? throw new ArgumentNullException(nameof(delayService));
                this.decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            }

            [DebuggerStepThrough]
            public async Task ExecuteAsync(TCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default(CancellationToken))
            {
                progress?.Report(new ProgressData("Intentionally delay the execution of the command."));
                await delayService.DelayAsync(command, ct).ConfigureAwait(false);
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

            public async Task DelayAsync<TCommand>(TCommand command, CancellationToken ct = default(CancellationToken))
                where TCommand : ICommand
            {
                var millisecondsDelay = random.Next(minMilliseconds, maxMilliseconds);
                await Task.Delay(millisecondsDelay, ct).ConfigureAwait(false);
            }
        }
    }
}
