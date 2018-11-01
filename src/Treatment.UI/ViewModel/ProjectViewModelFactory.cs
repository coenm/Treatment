namespace Treatment.UI.ViewModel
{
    using JetBrains.Annotations;

    using Treatment.Contract;
    using Treatment.Helpers;

    public class ProjectViewModelFactory : IProjectViewModelFactory
    {
        [NotNull] private readonly ICommandDispatcher commandDispatcher;

        public ProjectViewModelFactory([NotNull] ICommandDispatcher commandDispatcher)
        {
            this.commandDispatcher = Guard.NotNull(commandDispatcher, nameof(commandDispatcher));
        }

        public ProjectViewModel Create(string rootDirectoryInfoName, string rootDirectoryInfoFullName)
        {
            return new ProjectViewModel(
                                        rootDirectoryInfoName,
                                        rootDirectoryInfoFullName,
                                        commandDispatcher);
        }
    }
}
