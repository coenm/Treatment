namespace Treatment.Core.UseCases.CrossCuttingConcerns
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentValidation;

    using JetBrains.Annotations;

    using Treatment.Contract;
    using Treatment.Helpers;

    public class CommandValidationDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly IValidator<TCommand> validator;
        private readonly ICommandHandler<TCommand> decoratee;

        public CommandValidationDecorator([NotNull] IValidator<TCommand> validator, [NotNull] ICommandHandler<TCommand> decoratee)
        {
            this.validator = Guard.NotNull(validator, nameof(validator));
            this.decoratee = Guard.NotNull(decoratee, nameof(decoratee));
        }

        [DebuggerStepThrough]
        public Task ExecuteAsync(TCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default)
        {
            validator.ValidateAndThrow(command);

            return decoratee.ExecuteAsync(command, progress, ct);
        }
    }
}
