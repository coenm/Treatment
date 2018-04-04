namespace Treatment.Console.CrossCuttingConcerns
{
    using System;

    using Treatment.Core.UseCases;

    /// <summary>After successfully executing the command, the console will stay open (ie. Console.ReadKey())</summary>
    /// <typeparam name="TCommand">Command to execute</typeparam>
    public class HoldConsoleCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ITreatmentCommand
    {
        private readonly ICommandHandler<TCommand> _decorated;

        public HoldConsoleCommandHandlerDecorator(ICommandHandler<TCommand> decorated)
        {
            _decorated = decorated;
        }

        public void Execute(TCommand command)
        {
            _decorated.Execute(command);

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}