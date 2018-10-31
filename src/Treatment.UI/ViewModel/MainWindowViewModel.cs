namespace Treatment.UI.ViewModel
{
    using System;

    using JetBrains.Annotations;
    using Nito.Mvvm;
    using Treatment.Contract;
    using Treatment.Helpers;
    using Treatment.UI.Core.Configuration;
    using Treatment.UI.Framework;

    using ICommand = System.Windows.Input.ICommand;

    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel, IInitializableViewModel
    {
        [NotNull] private readonly IProgress<ProgressData> progressFixCsProjectFiles;

        public MainWindowViewModel(
            [NotNull] IProjectCollectionViewModel projectCollectionViewModel,
            [NotNull] IConfigurationService configurationService,
            [NotNull] IShowEntityInDialogProcessor showInDialogProcessor)
        {
            Guard.NotNull(configurationService, nameof(configurationService));
            Guard.NotNull(showInDialogProcessor, nameof(showInDialogProcessor));
            ProjectCollection = Guard.NotNull(projectCollectionViewModel, nameof(projectCollectionViewModel));

            progressFixCsProjectFiles = new Progress<ProgressData>(data =>
            {
                if (string.IsNullOrEmpty(data.Message))
                    return;

                // THIS IS PROBABLY NOT THE WAY TO DO THIS..
                FixCsProjectFilesLog += data.Message + Environment.NewLine;
            });

            WorkingDirectory = configurationService.GetConfiguration().RootPath ?? string.Empty;

            OpenSettings = new OpenSettingsCommand(showInDialogProcessor, configurationService);
            Initialize = ProjectCollection.Initialize;
        }

        public IProjectCollectionViewModel ProjectCollection { get; }

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
            [NotNull] private readonly IShowEntityInDialogProcessor showEntityInDialogProcessor;
            [NotNull] private readonly IConfigurationService configurationService;

            public OpenSettingsCommand(
                [NotNull] IShowEntityInDialogProcessor showEntityInDialogProcessor,
                [NotNull] IConfigurationService configurationService)
            {
                this.showEntityInDialogProcessor = Guard.NotNull(showEntityInDialogProcessor, nameof(showEntityInDialogProcessor));
                this.configurationService = Guard.NotNull(configurationService, nameof(configurationService));
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
