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
    /// For testing purposes
    /// </summary>
    public static class DelayCommandExecution
    {
        public static void Register([NotNull] Container container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            container.Register<IDelayService>(() => new RandomDelayService(1000, 5000), Lifestyle.Singleton);

            container.RegisterDecorator(
                                        typeof(ICommandHandler<>),
                                        typeof(CommandDelayDecorator<>),
                                        Lifestyle.Scoped);
        }

        private class CommandDelayDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
        {
            private readonly ICommandHandler<TCommand> _decoratee;
            private readonly IDelayService _delayService;

            public CommandDelayDecorator([NotNull] IDelayService delayService, [NotNull] ICommandHandler<TCommand> decoratee)
            {
                _delayService = delayService ?? throw new ArgumentNullException(nameof(delayService));
                _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            }

            [DebuggerStepThrough]
            public async Task ExecuteAsync(TCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default(CancellationToken))
            {
                progress?.Report(new ProgressData("Intentionally delay the execution of the command."));
                await _delayService.DelayAsync(command, ct).ConfigureAwait(false);
                progress?.Report(new ProgressData("Delayed the execution enough.."));

                await _decoratee.ExecuteAsync(command, progress, ct).ConfigureAwait(false);
            }
        }

        private interface IDelayService
        {
            Task DelayAsync<TCommand>([NotNull] TCommand command, CancellationToken ct = default(CancellationToken)) where TCommand : ICommand;
        }

        private class RandomDelayService : IDelayService
        {
            private readonly int _minMilliseconds;
            private readonly int _maxMilliseconds;
            [NotNull] private readonly Random _random;

            public RandomDelayService(int minMilliseconds, int maxMilliseconds)
            {
                _minMilliseconds = minMilliseconds;
                _maxMilliseconds = maxMilliseconds;
                _random = new Random();
            }

            public async Task DelayAsync<TCommand>(TCommand command, CancellationToken ct = default(CancellationToken)) where TCommand : ICommand
            {
                var millisecondsDelay = _random.Next(_minMilliseconds, _maxMilliseconds);
                await Task.Delay(millisecondsDelay, ct).ConfigureAwait(false);
            }
        }
    }
}