namespace Treatment.UI.ViewModel
{
    using System;

    using JetBrains.Annotations;

    using Nito.Mvvm;

    using Treatment.Contract;
    using Treatment.Contract.Commands;
    using Treatment.UI.Core;

    public class ProjectViewModel : ViewModelBase, IDisposable
    {
        [NotNull] private readonly ExecutingAsyncCommandsComposition commandWatch;

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
            FixCsProjectFiles = new CapturingExceptionAsyncCommand(
                                                                   async _ => await handlerUpdateProjectFilesCommand.ExecuteAsync(new UpdateProjectFilesCommand(Path)),
                                                                   _ => TaskRunning == false);

            RemoveNewAppConfig = new CapturingExceptionAsyncCommand(
                                                                    async _ => await handlerCleanAppConfigCommand.ExecuteAsync(new CleanAppConfigCommand(Path)),
                                                                    _ => TaskRunning == false);

            commandWatch = new ExecutingAsyncCommandsComposition();
            commandWatch.WatchCommand(FixCsProjectFiles);
            commandWatch.WatchCommand(RemoveNewAppConfig);
            commandWatch.RegisterAction(value => TaskRunning = value);
        }

        public bool TaskRunning
        {
            get => Properties.Get(false);
            set => Properties.Set(value);
        }

        public string Name { get; }

        public string Path { get; }

        [UsedImplicitly]
        public CapturingExceptionAsyncCommand FixCsProjectFiles { get; }

        [UsedImplicitly]
        public CapturingExceptionAsyncCommand RemoveNewAppConfig { get; }

        public void Dispose()
        {
            commandWatch.Dispose();
        }
    }
}
