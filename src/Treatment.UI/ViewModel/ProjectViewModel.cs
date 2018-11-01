namespace Treatment.UI.ViewModel
{
    using System;

    using JetBrains.Annotations;
    using Nito.Mvvm;
    using Treatment.Contract;
    using Treatment.Contract.Commands;
    using Treatment.Helpers;
    using Treatment.UI.Framework;
    using Treatment.UI.Framework.ViewModel;

    public class ProjectViewModel : ViewModelBase, IDisposable
    {
        [NotNull] private readonly ExecutingAsyncCommandsComposition commandWatch;

        public ProjectViewModel(string name,
                                string path,
                                [NotNull] ICommandDispatcher commandDispatcher)
        {
            Guard.NotNull(commandDispatcher, nameof(commandDispatcher));
            Guard.NotNullOrWhiteSpace(name, nameof(name));
            Guard.NotNullOrWhiteSpace(path, nameof(path));

            Name = name;
            Path = path;
            FixCsProjectFiles = new CapturingExceptionAsyncCommand(
                async _ => await commandDispatcher.ExecuteAsync(new UpdateProjectFilesCommand(Path)),
                _ => TaskRunning == false);

            RemoveNewAppConfig = new CapturingExceptionAsyncCommand(
                async _ => await commandDispatcher.ExecuteAsync(new CleanAppConfigCommand(Path)),
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
