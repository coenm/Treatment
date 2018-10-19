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
        [NotNull] private readonly ICommandHandler<UpdateProjectFilesCommand> _handlerUpdateProjectFilesCommand;
        [NotNull] private readonly ICommandHandler<CleanAppConfigCommand> _handlerCleanAppConfigCommand;
        [NotNull] private readonly IFileSearch _fileSearch;
        [NotNull] private readonly IConfiguration _configuration;
        [NotNull] private readonly IProgress<ProgressData> _progressFixCsProjectFiles;

        public MainWindowViewModel(
            [NotNull] ICommandHandler<UpdateProjectFilesCommand> handlerUpdateProjectFilesCommand,
            [NotNull] ICommandHandler<CleanAppConfigCommand> handlerCleanAppConfigCommand,
            [NotNull] IFileSearch fileSearch,
            [NotNull] IConfiguration configuration,
            [NotNull] IShowEntityInDialogProcessor showInDialogProcessor)
        {
            _progressFixCsProjectFiles = new Progress<ProgressData>(data =>
                                                                    {
                                                                        if (string.IsNullOrEmpty(data.Message))
                                                                            return;

                                                                        // THIS IS PROBABLY NOT THE WAY TO DO THIS..
                                                                        FixCsProjectFilesLog += data.Message + Environment.NewLine;
                                                                    });

            _handlerUpdateProjectFilesCommand = handlerUpdateProjectFilesCommand ?? throw new ArgumentNullException(nameof(handlerUpdateProjectFilesCommand));
            _handlerCleanAppConfigCommand = handlerCleanAppConfigCommand ?? throw new ArgumentNullException(nameof(handlerCleanAppConfigCommand));
            _fileSearch = fileSearch ?? throw new ArgumentNullException(nameof(fileSearch));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            if (showInDialogProcessor == null)
                throw new ArgumentNullException(nameof(showInDialogProcessor));

            WorkingDirectory = _configuration.RootPath ?? string.Empty;

            ProjectCollection = new ProjectCollectionViewModel(_handlerUpdateProjectFilesCommand, _handlerCleanAppConfigCommand, _fileSearch, _configuration);
            OpenSettings = new OpenSettingsCommand(showInDialogProcessor, _configuration.RootPath ?? string.Empty);
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
            private readonly IShowEntityInDialogProcessor _showEntityInDialogProcessor;
            [NotNull] private readonly string _rootPath;

            public OpenSettingsCommand([NotNull] IShowEntityInDialogProcessor showEntityInDialogProcessor, [NotNull] string rootPath)
            {
                _showEntityInDialogProcessor = showEntityInDialogProcessor ?? throw new ArgumentNullException(nameof(showEntityInDialogProcessor));
                _rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
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
                                                  RootDirectory = _rootPath
                                              };

                var result = _showEntityInDialogProcessor.ShowDialog(applicationSettings);
                if (result == true)
                {
                }
            }
        }
    }
}