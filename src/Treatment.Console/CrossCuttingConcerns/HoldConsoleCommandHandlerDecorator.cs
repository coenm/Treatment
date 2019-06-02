namespace Treatment.Console.CrossCuttingConcerns
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Treatment.Contract;

    /// <summary>After successfully executing the command, the console will stay open (ie. Console.ReadKey()).</summary>
    /// <typeparam name="TCommand">Command to execute.</typeparam>
    public class HoldConsoleCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> decorated;
        private readonly IHoldConsole holdConsole;

        public HoldConsoleCommandHandlerDecorator(
            [NotNull] ICommandHandler<TCommand> decorated,
            [NotNull] IHoldConsole holdConsole)
        {
            this.decorated = decorated;
            this.holdConsole = holdConsole;
        }

        public async Task ExecuteAsync(TCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default)
        {
            await decorated.ExecuteAsync(command, progress, ct).ConfigureAwait(false);
            holdConsole.Hold();
        }
    }
}
