namespace Treatment.Console.CrossCuttingConcerns
{
    using System.Threading.Tasks;

    using Treatment.Contract;
    using Treatment.Contract.Commands;

    internal class SetRootDirectoryCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly IRootDirSanitizer _rootDirSanitizer;
        private readonly ICommandHandler<TCommand> _decoratee;

        public SetRootDirectoryCommandHandlerDecorator(IRootDirSanitizer rootDirSanitizer, ICommandHandler<TCommand> decoratee)
        {
            _rootDirSanitizer = rootDirSanitizer;
            _decoratee = decoratee;
        }

        public async Task ExecuteAsync(TCommand command)
        {
            if (command is IDirectoryProperty directoryCommand)
                _rootDirSanitizer.SetRootDir(directoryCommand.Directory);

            await _decoratee.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}