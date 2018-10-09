namespace Treatment.UI.ViewModel
{
    using System;

    using JetBrains.Annotations;

    using Nito.Mvvm;

    using Treatment.Contract;
    using Treatment.Contract.Commands;

    public class ProjectViewModel : ViewModelBase
    {
        public ProjectViewModel(string name, string path, [NotNull] ICommandHandler<UpdateProjectFilesCommand> commandHandler)
        {
            if (commandHandler == null)
                throw new ArgumentNullException(nameof(commandHandler));

            Name = name;
            Path = path;
            FixCsProjectFiles = new CapturingExceptionAsyncCommand(async _ => await commandHandler.ExecuteAsync(new UpdateProjectFilesCommand(Path)));
            // FixCsProjectFiles = new CapturingExceptionAsyncCommand(async _ => await commandHandler.ExecuteAsync(new UpdateProjectFilesCommand("D:\\Users\\coen\\Downloads\\temp\\")));
        }

        public string Name { get; }

        public string Path { get; }

        [UsedImplicitly]
        public CapturingExceptionAsyncCommand FixCsProjectFiles { get; }
    }
}