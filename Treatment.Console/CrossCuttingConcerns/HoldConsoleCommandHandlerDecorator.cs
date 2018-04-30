namespace Treatment.Console.CrossCuttingConcerns
{
    using Treatment.Console.Console;
    using Treatment.Contract;

    /// <summary>After successfully executing the command, the console will stay open (ie. Console.ReadKey())</summary>
    /// <typeparam name="TCommand">Command to execute</typeparam>
    public class HoldConsoleCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _decorated;
        private readonly IConsole _console;

        public HoldConsoleCommandHandlerDecorator(ICommandHandler<TCommand> decorated, IConsole console)
        {
            _decorated = decorated;
            _console = console;
        }

        public void Execute(TCommand command)
        {
            _decorated.Execute(command);

            _console.WriteLine("Press enter to exit");
            _console.ReadLine();
        }
    }
}