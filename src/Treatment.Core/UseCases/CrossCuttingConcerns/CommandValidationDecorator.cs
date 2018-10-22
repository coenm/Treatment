namespace Treatment.Core.UseCases.CrossCuttingConcerns
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentValidation;

    using JetBrains.Annotations;

    using Treatment.Contract;

    public class CommandValidationDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly IValidator<TCommand> _validator;
        private readonly ICommandHandler<TCommand> _decoratee;

        public CommandValidationDecorator([NotNull] IValidator<TCommand> validator, [NotNull] ICommandHandler<TCommand> decoratee)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        [DebuggerStepThrough]
        public Task ExecuteAsync(TCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default(CancellationToken))
        {
            _validator.ValidateAndThrow(command);

            return _decoratee.ExecuteAsync(command, progress, ct);
        }
    }
}