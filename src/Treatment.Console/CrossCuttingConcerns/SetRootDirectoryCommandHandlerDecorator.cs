namespace Treatment.Console.CrossCuttingConcerns
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Treatment.Contract;
    using Treatment.Contract.Commands;

    internal class SetRootDirectoryCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly IRootDirSanitizer rootDirSanitizer;
        private readonly ICommandHandler<TCommand> decoratee;

        public SetRootDirectoryCommandHandlerDecorator(IRootDirSanitizer rootDirSanitizer, ICommandHandler<TCommand> decoratee)
        {
            this.rootDirSanitizer = rootDirSanitizer;
            this.decoratee = decoratee;
        }

        public async Task ExecuteAsync(TCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default(CancellationToken))
        {
            if (command is IDirectoryProperty directoryCommand)
                rootDirSanitizer.SetRootDir(directoryCommand.Directory);

            await decoratee.ExecuteAsync(command, progress, ct).ConfigureAwait(false);
        }
    }
}
