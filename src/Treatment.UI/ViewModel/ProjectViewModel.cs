namespace Treatment.UI.ViewModel
{
    using System;

    using JetBrains.Annotations;

    using Nito.Mvvm;

    using Treatment.Contract;
    using Treatment.Contract.Commands;

    public class ProjectViewModel : ViewModelBase
    {
        public ProjectViewModel(
            string name,
            string path,
            [NotNull] ICommandHandler<UpdateProjectFilesCommand> handlerUpdateProjectFilesCommand,
            [NotNull] ICommandHandler<CleanAppConfigCommand> handlerCleanAppConfigCommand)
        {
            if (handlerUpdateProjectFilesCommand == null)
                throw new ArgumentNullException(nameof(handlerUpdateProjectFilesCommand));
            if (handlerCleanAppConfigCommand == null)
                throw new ArgumentNullException(nameof(handlerCleanAppConfigCommand));

            Name = name;
            Path = path;
            FixCsProjectFiles = new CapturingExceptionAsyncCommand(async _ => await handlerUpdateProjectFilesCommand.ExecuteAsync(new UpdateProjectFilesCommand(Path)));
            RemoveNewAppConfig = new CapturingExceptionAsyncCommand(async _ => await handlerCleanAppConfigCommand.ExecuteAsync(new CleanAppConfigCommand(Path)));
        }

        public string Name { get; }

        public string Path { get; }

        [UsedImplicitly]
        public CapturingExceptionAsyncCommand FixCsProjectFiles { get; }

        [UsedImplicitly]
        public CapturingExceptionAsyncCommand RemoveNewAppConfig { get; }
    }
}