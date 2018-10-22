namespace Treatment.UI.ViewModel
{
    using System;

    using JetBrains.Annotations;

    using Treatment.Contract;
    using Treatment.Contract.Commands;
    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.UI.Core;
    using Treatment.UI.Core.UI;
    using Treatment.UI.ViewModel.Settings;

    using ICommand = System.Windows.Input.ICommand;

    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        [NotNull] private readonly ICommandHandler<UpdateProjectFilesCommand> handlerUpdateProjectFilesCommand;
        [NotNull] private readonly ICommandHandler<CleanAppConfigCommand> handlerCleanAppConfigCommand;
        [NotNull] private readonly IFileSearch fileSearch;
        [NotNull] private readonly IConfiguration configuration;
        [NotNull] private readonly IProgress<ProgressData> progressFixCsProjectFiles;

        public MainWindowViewModel(
            [NotNull] ICommandHandler<UpdateProjectFilesCommand> handlerUpdateProjectFilesCommand,
            [NotNull] ICommandHandler<CleanAppConfigCommand> handlerCleanAppConfigCommand,
            [NotNull] IFileSearch fileSearch,
            [NotNull] IConfiguration configuration,
            [NotNull] IShowEntityInDialogProcessor showInDialogProcessor)
        {
            progressFixCsProjectFiles = new Progress<ProgressData>(data =>
                                                                    {
                                                                        if (string.IsNullOrEmpty(data.Message))
                                                                            return;

                                                                        // THIS IS PROBABLY NOT THE WAY TO DO THIS..
                                                                        FixCsProjectFilesLog += data.Message + Environment.NewLine;
                                                                    });

            this.handlerUpdateProjectFilesCommand = handlerUpdateProjectFilesCommand ?? throw new ArgumentNullException(nameof(handlerUpdateProjectFilesCommand));
            this.handlerCleanAppConfigCommand = handlerCleanAppConfigCommand ?? throw new ArgumentNullException(nameof(handlerCleanAppConfigCommand));
            this.fileSearch = fileSearch ?? throw new ArgumentNullException(nameof(fileSearch));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            if (showInDialogProcessor == null)
                throw new ArgumentNullException(nameof(showInDialogProcessor));

            WorkingDirectory = this.configuration.RootPath ?? string.Empty;

            ProjectCollection = new ProjectCollectionViewModel(this.handlerUpdateProjectFilesCommand, this.handlerCleanAppConfigCommand, this.fileSearch, this.configuration);
            OpenSettings = new OpenSettingsCommand(showInDialogProcessor, this.configuration.RootPath ?? string.Empty);
        }

        public ProjectCollectionViewModel ProjectCollection { get; }

        public ICommand OpenSettings { get; }

        public string FixCsProjectFilesLog
        {
            get => Properties.Get(string.Empty);
            set => Properties.Set(value);
        }

        public string WorkingDirectory
        {
            get => Properties.Get(string.Empty);
            set => Properties.Set(value);
        }

        public ProgressData? InProgress
        {
            get => Properties.Get<ProgressData>(null);
            set => Properties.Set(value);
        }

        private class OpenSettingsCommand : ICommand
        {
            private readonly IShowEntityInDialogProcessor showEntityInDialogProcessor;
            [NotNull] private readonly string rootPath;

            public OpenSettingsCommand([NotNull] IShowEntityInDialogProcessor showEntityInDialogProcessor, [NotNull] string rootPath)
            {
                showEntityInDialogProcessor = showEntityInDialogProcessor ?? throw new ArgumentNullException(nameof(showEntityInDialogProcessor));
                rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                var applicationSettings = new ApplicationSettings
                                              {
                                                  DelayExecution = true,
                                                  SearchProviderName = "FileSystem",
                                                  VersionControlProviderName = "SVN",
                                                  RootDirectory = rootPath,
                                              };

                var result = showEntityInDialogProcessor.ShowDialog(applicationSettings);
                if (result == true)
                {
                }
            }
        }
    }
}
