namespace Treatment.UI.Core.ViewModel
{
    using Contract;
    using Helpers.Guards;
    using JetBrains.Annotations;

    public class ProjectViewModelFactory : IProjectViewModelFactory
    {
        [NotNull] private readonly ICommandDispatcher commandDispatcher;

        public ProjectViewModelFactory([NotNull] ICommandDispatcher commandDispatcher)
        {
            Guard.NotNull(commandDispatcher, nameof(commandDispatcher));
            this.commandDispatcher = commandDispatcher;
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
