namespace Treatment.Console.CrossCuttingConcerns
{
    using JetBrains.Annotations;

    using Treatment.Console.Console;
    using Treatment.Contract;

    /// <summary>After successfully executing the command, the console will stay open (ie. Console.ReadKey())</summary>
    /// <typeparam name="TCommand">Command to execute</typeparam>
    public class HoldConsoleCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _decorated;
        private readonly IHoldConsole _console;

        public HoldConsoleCommandHandlerDecorator(
            [NotNull] ICommandHandler<TCommand> decorated,
            [NotNull] IHoldConsole console)
        {
            _decorated = decorated;
            _console = console;
        }

        public void Execute(TCommand command)
        {
            _decorated.Execute(command);
            _console.Hold();
        }
    }

    public class HoldConsole : IHoldConsole
    {
        [NotNull]
        private readonly IConsole _console;

        public HoldConsole([NotNull] IConsole console)
        {
            _console = console;
        }

        public void Hold()
        {
            _console.WriteLine("Press enter to exit");
            _console.ReadLine();
        }
    }

    public interface IHoldConsole
    {
        void Hold();
    }
}