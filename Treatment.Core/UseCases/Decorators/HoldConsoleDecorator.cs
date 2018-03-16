namespace Treatment.Core.UseCases.Decorators
{
    using System;

    public class HoldConsoleDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ITreatmentCommand
    {
        private readonly ICommandHandler<TCommand> _decorated;

        public HoldConsoleDecorator(ICommandHandler<TCommand> decorated)
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