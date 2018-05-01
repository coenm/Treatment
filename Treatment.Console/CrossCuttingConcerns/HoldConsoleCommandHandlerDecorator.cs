namespace Treatment.Console.CrossCuttingConcerns
{
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

        public void Execute(TCommand command)
        {
            _decorated.Execute(command);
            _holdConsole.Hold();
        }
    }
}