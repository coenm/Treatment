namespace Treatment.Core.UseCases.CrossCuttingConcerns
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentValidation;

    using JetBrains.Annotations;

    using Treatment.Contract;
    using Treatment.Helpers.Guards;

    public class CommandValidationDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly IValidator<TCommand> validator;
        private readonly ICommandHandler<TCommand> decoratee;

        public CommandValidationDecorator([NotNull] IValidator<TCommand> validator, [NotNull] ICommandHandler<TCommand> decoratee)
        {
            Guard.NotNull(validator, nameof(validator));
            Guard.NotNull(decoratee, nameof(decoratee));
            this.validator = validator;
            this.decoratee = decoratee;
        }

        [DebuggerStepThrough]
        public Task ExecuteAsync(TCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default(CancellationToken))
        {
            validator.ValidateAndThrow(command);

            return decoratee.ExecuteAsync(command, progress, ct);
        }
    }
}
