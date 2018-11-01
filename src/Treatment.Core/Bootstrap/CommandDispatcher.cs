namespace Treatment.Core.Bootstrap
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using SimpleInjector;

    using Treatment.Contract;
    using Treatment.Helpers;

    [UsedImplicitly]
    public class CommandDispatcher : ICommandDispatcher
    {
        [NotNull] private readonly Container container;

        public CommandDispatcher([NotNull] Container container)
        {
            this.container = Guard.NotNull(container, nameof(container));
        }

        public async Task ExecuteAsync<TCommand>(TCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default)
            where TCommand : class, ICommand
        {
            Guard.NotNull(command, nameof(command));

            var commandHandler = container.GetInstance<ICommandHandler<TCommand>>();

            await commandHandler.ExecuteAsync(command, null, ct).ConfigureAwait(false);
        }
    }
}
