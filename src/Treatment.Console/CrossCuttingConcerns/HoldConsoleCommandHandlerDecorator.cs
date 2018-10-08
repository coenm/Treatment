namespace Treatment.Console.CrossCuttingConcerns
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Treatment.Contract;

    /// <summary>After successfully executing the command, the console will stay open (ie. Console.ReadKey())</summary>
    /// <typeparam name="TCommand">Command to execute</typeparam>
    public class HoldConsoleCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _decorated;
        private readonly IHoldConsole _holdConsole;

        public HoldConsoleCommandHandlerDecorator(
            [NotNull] ICommandHandler<TCommand> decorated,
            [NotNull] IHoldConsole holdConsole)
        {
            _decorated = decorated;
            _holdConsole = holdConsole;
        }

        public async Task ExecuteAsync(TCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default(CancellationToken))
        {
            await _decorated.ExecuteAsync(command, progress, ct).ConfigureAwait(false);
            _holdConsole.Hold();
        }
    }
}