namespace Treatment.Console.CrossCuttingConcerns
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Treatment.Console.Console;
    using Treatment.Contract;

    /// <summary>Catch, and write exception message to console.</summary>
    /// <typeparam name="TCommand">Command to handle</typeparam>
    [UsedImplicitly]
    public class WriteExceptionToConsoleCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _decorated;
        private readonly IConsole _console;

        public WriteExceptionToConsoleCommandHandlerDecorator(ICommandHandler<TCommand> decorated, IConsole console)
        {
            _decorated = decorated;
            _console = console;
        }

        public async Task ExecuteAsync(TCommand command)
        {
            try
            {
                await _decorated.ExecuteAsync(command).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _console.WriteLine(e.Message);

                _console.WriteLine();
                _console.WriteLine("Press enter to continue");
                _console.ReadLine();
            }
        }
    }
}