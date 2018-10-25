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
        private readonly IValidator<TCommand> validator;
        private readonly ICommandHandler<TCommand> decoratee;

        public CommandValidationDecorator([NotNull] IValidator<TCommand> validator, [NotNull] ICommandHandler<TCommand> decoratee)
        {
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
            this.decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        [DebuggerStepThrough]
        public Task ExecuteAsync(TCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default)
        {
            validator.ValidateAndThrow(command);

            return decoratee.ExecuteAsync(command, progress, ct);
        }
    }
}
