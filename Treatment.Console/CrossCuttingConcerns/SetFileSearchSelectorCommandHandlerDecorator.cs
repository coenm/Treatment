namespace Treatment.Console.CrossCuttingConcerns
{
    using Treatment.Contract;
    using Treatment.Contract.Commands;
    using Treatment.Core.DefaultPluginImplementation;
    using Treatment.Core.DefaultPluginImplementation.FileSearch;

    internal class SetFileSearchSelectorCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly IFileSearchSelector _fileSearchSelector;
        private readonly ICommandHandler<TCommand> _decoratee;

        public SetFileSearchSelectorCommandHandlerDecorator(IFileSearchSelector fileSearchSelector, ICommandHandler<TCommand> decoratee)
        {
            _fileSearchSelector = fileSearchSelector;
            _decoratee = decoratee;
        }

        public void Execute(TCommand command)
        {
            if (command is IDirectoryProperty directoryCommand)
                _fileSearchSelector.SetRequestedSearchProvider(directoryCommand.Directory);

            _decoratee.Execute(command);
        }
    }
}