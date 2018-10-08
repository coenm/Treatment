namespace Treatment.Core.UseCases.CrossCuttingConcerns
{
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentValidation;

    using Treatment.Contract;

    public class CommandValidationDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly IValidator<TCommand> _validator;
        private readonly ICommandHandler<TCommand> _decoratee;

        public CommandValidationDecorator(IValidator<TCommand> validator, ICommandHandler<TCommand> decoratee)
        {
            _validator = validator;
            _decoratee = decoratee;
        }

        [DebuggerStepThrough]
        public Task ExecuteAsync(TCommand command, CancellationToken ct = default(CancellationToken))
        {
            _validator.ValidateAndThrow(command);

            return _decoratee.ExecuteAsync(command, ct);
        }
    }
}