namespace Treatment.Core.UseCases.CrossCuttingConcerns
{
    using System.Diagnostics;

    using FluentValidation;

    public class CommandValidationDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ITreatmentCommand
    {
        private readonly IValidator<TCommand> _validator;
        private readonly ICommandHandler<TCommand> _decoratee;

        public CommandValidationDecorator(IValidator<TCommand> validator, ICommandHandler<TCommand> decoratee)
        {
            _validator = validator;
            _decoratee = decoratee;
        }

        [DebuggerStepThrough]
        public void Execute(TCommand command)
        {
            _validator.ValidateAndThrow(command);

            _decoratee.Execute(command);
        }
    }
}