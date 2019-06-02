namespace Treatment.Console.CrossCuttingConcerns
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Treatment.Console.Console;
    using Treatment.Contract;

    /// <summary>Catch, and write exception message to console.</summary>
    /// <typeparam name="TCommand">Command to handle.</typeparam>
    [UsedImplicitly]
    public class WriteExceptionToConsoleCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> decorated;
        private readonly IConsole console;

        public WriteExceptionToConsoleCommandHandlerDecorator(ICommandHandler<TCommand> decorated, IConsole console)
        {
            this.decorated = decorated;
            this.console = console;
        }

        public async Task ExecuteAsync(TCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default)
        {
            try
            {
                await decorated.ExecuteAsync(command, progress, ct).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                console.WriteLine(e.Message);

                console.WriteLine();
                console.WriteLine("Press enter to continue");
                console.ReadLine();
            }
        }
    }
}
