namespace Treatment.Console.CrossCuttingConcerns
{
    using System;

    using JetBrains.Annotations;

    using Treatment.Core.UseCases;

    /// <summary>Catch, and write exception message to console.</summary>
    /// <typeparam name="TCommand">Command to handle</typeparam>
    [UsedImplicitly]
    public class WriteExceptionToConsoleCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ITreatmentCommand
    {
        private readonly ICommandHandler<TCommand> _decorated;

        public WriteExceptionToConsoleCommandHandlerDecorator(ICommandHandler<TCommand> decorated)
        {
            _decorated = decorated;
        }

        public void Execute(TCommand command)
        {
            try
            {
                _decorated.Execute(command);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                Console.WriteLine();
                Console.WriteLine("Press enter to continue");
                Console.ReadLine();
            }
        }
    }
}