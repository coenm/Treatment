namespace Treatment.Console.CrossCuttingConcerns
{
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

        public void Execute(TCommand command)
        {
            if (command is IDirectoryProperty directoryCommand)
                _rootDirSanitizer.SetRootDir(directoryCommand.Directory);

            _decoratee.Execute(command);
        }
    }
}