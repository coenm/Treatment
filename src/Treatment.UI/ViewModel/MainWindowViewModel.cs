namespace Treatment.UI.ViewModel
{
    using System;

    using JetBrains.Annotations;
    using Nito.Mvvm;
    using Treatment.Contract;
    using Treatment.Contract.Commands;
    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.Helpers;
    using Treatment.UI.Core.Configuration;
    using Treatment.UI.Core.UI;

    using ICommand = System.Windows.Input.ICommand;

    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel, IInitializableViewModel
    {
        [NotNull] private readonly IProgress<ProgressData> progressFixCsProjectFiles;

        public MainWindowViewModel(
            [NotNull] ICommandHandler<UpdateProjectFilesCommand> handlerUpdateProjectFilesCommand,
            [NotNull] ICommandHandler<CleanAppConfigCommand> handlerCleanAppConfigCommand,
            [NotNull] IFileSearch fileSearch,
            [NotNull] IConfigurationService configurationService,
            [NotNull] IShowEntityInDialogProcessor showInDialogProcessor)
        {
            Guard.NotNull(handlerUpdateProjectFilesCommand, nameof(handlerUpdateProjectFilesCommand));
            Guard.NotNull(handlerCleanAppConfigCommand, nameof(handlerCleanAppConfigCommand));
            Guard.NotNull(fileSearch, nameof(fileSearch));
            Guard.NotNull(configurationService, nameof(configurationService));
            Guard.NotNull(showInDialogProcessor, nameof(showInDialogProcessor));

            progressFixCsProjectFiles = new Progress<ProgressData>(data =>
            {
                if (string.IsNullOrEmpty(data.Message))
                    return;

                // THIS IS PROBABLY NOT THE WAY TO DO THIS..
                FixCsProjectFilesLog += data.Message + Environment.NewLine;
            });

            WorkingDirectory = configurationService.GetConfiguration().RootPath ?? string.Empty;

            ProjectCollection = new ProjectCollectionViewModel(
                handlerUpdateProjectFilesCommand,
                handlerCleanAppConfigCommand,
                fileSearch,
                configurationService.GetConfiguration());

            OpenSettings = new OpenSettingsCommand(showInDialogProcessor, configurationService);
            Initialize = ProjectCollection.Initialize;
        }

        public ProjectCollectionViewModel ProjectCollection { get; }

        public ICommand OpenSettings { get; }

        ICommand IInitializableViewModel.Initialize => Initialize;

        public CapturingExceptionAsyncCommand Initialize { get; }

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
            [NotNull] private readonly IConfigurationService configurationService;

            public OpenSettingsCommand(
                [NotNull] IShowEntityInDialogProcessor showEntityInDialogProcessor,
                [NotNull] IConfigurationService configurationService)
            {
                this.showEntityInDialogProcessor = showEntityInDialogProcessor ?? throw new ArgumentNullException(nameof(showEntityInDialogProcessor));
                this.configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
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
                                                  RootDirectory = configurationService.GetConfiguration().RootPath,
                                              };

                var result = showEntityInDialogProcessor.ShowDialog(applicationSettings);
                if (result == true)
                {
                    // todo
                    configurationService.UpdateAsync(applicationSettings);
                }
            }
        }
    }
}
