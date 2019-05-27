namespace Treatment.Core.Bootstrap
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using SimpleInjector;

    using Treatment.Contract;
    using Treatment.Helpers.Guards;

    [UsedImplicitly]
    public class CommandDispatcher : ICommandDispatcher
    {
        [NotNull] private readonly Container container;

        public CommandDispatcher([NotNull] Container container)
        {
            Guard.NotNull(container, nameof(container));
            this.container = container;
        }

        public async Task ExecuteAsync<TCommand>(TCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default(CancellationToken))
            where TCommand : class, ICommand
        {
            Guard.NotNull(command, nameof(command));

            try
            {
                var commandHandler = container.GetInstance<ICommandHandler<TCommand>>();
                await commandHandler.ExecuteAsync(command, progress, ct).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
