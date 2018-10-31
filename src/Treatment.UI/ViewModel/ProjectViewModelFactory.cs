namespace Treatment.UI.ViewModel
{
    using JetBrains.Annotations;

    using Treatment.Contract;
    using Treatment.Contract.Commands;
    using Treatment.Helpers;

    public class ProjectViewModelFactory : IProjectViewModelFactory
    {
        [NotNull] private readonly ICommandHandler<UpdateProjectFilesCommand> handlerUpdateProjectFilesCommand;
        [NotNull] private readonly ICommandHandler<CleanAppConfigCommand> handlerCleanAppConfigCommand;

        public ProjectViewModelFactory([NotNull] ICommandHandler<UpdateProjectFilesCommand> handlerUpdateProjectFilesCommand, [NotNull] ICommandHandler<CleanAppConfigCommand> handlerCleanAppConfigCommand)
        {
            this.handlerUpdateProjectFilesCommand = Guard.NotNull(handlerUpdateProjectFilesCommand, nameof(handlerUpdateProjectFilesCommand));
            this.handlerCleanAppConfigCommand = Guard.NotNull(handlerCleanAppConfigCommand, nameof(handlerCleanAppConfigCommand));
        }

        public ProjectViewModel Create(string rootDirectoryInfoName, string rootDirectoryInfoFullName)
        {
            return new ProjectViewModel(
                                        rootDirectoryInfoName,
                                        rootDirectoryInfoFullName,
                                        handlerUpdateProjectFilesCommand,
                                        handlerCleanAppConfigCommand);
        }
    }
}
