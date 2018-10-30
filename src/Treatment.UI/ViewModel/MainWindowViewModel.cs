using Nito.Mvvm;
using Treatment.UI.Framework;

namespace Treatment.UI.ViewModel
{
    using System;

    using JetBrains.Annotations;
    using Treatment.Contract;
    using Treatment.Contract.Commands;
    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.UI.Core.Configuration;
    using Treatment.UI.Core.UI;

    using ICommand = System.Windows.Input.ICommand;

    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        [NotNull] private readonly ICommandHandler<UpdateProjectFilesCommand> handlerUpdateProjectFilesCommand;
        [NotNull] private readonly ICommandHandler<CleanAppConfigCommand> handlerCleanAppConfigCommand;
        [NotNull] private readonly IFileSearch fileSearch;
        [NotNull] private readonly IConfigurationService configurationService;
        [NotNull] private readonly IProgress<ProgressData> progressFixCsProjectFiles;

        public MainWindowViewModel(
            [NotNull] ICommandHandler<UpdateProjectFilesCommand> handlerUpdateProjectFilesCommand,
            [NotNull] ICommandHandler<CleanAppConfigCommand> handlerCleanAppConfigCommand,
            [NotNull] IFileSearch fileSearch,
            [NotNull] IConfigurationService configurationService,
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
            this.configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
            if (showInDialogProcessor == null)
                throw new ArgumentNullException(nameof(showInDialogProcessor));

            WorkingDirectory = configurationService.GetConfiguration().RootPath ?? string.Empty;

            ProjectCollection = new ProjectCollectionViewModel(
                this.handlerUpdateProjectFilesCommand,
                this.handlerCleanAppConfigCommand,
                this.fileSearch,
                this.configurationService.GetConfiguration());

            OpenSettings = new OpenSettingsCommand(showInDialogProcessor, configurationService);
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
