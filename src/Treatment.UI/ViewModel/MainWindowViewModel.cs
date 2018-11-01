﻿namespace Treatment.UI.ViewModel
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Nito.Mvvm;
    using Treatment.Contract;
    using Treatment.Helpers;
    using Treatment.UI.Core.Configuration;
    using Treatment.UI.Framework;
    using Treatment.UI.Framework.ViewModel;

    using ICommand = System.Windows.Input.ICommand;

    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel, IInitializableViewModel
    {
        [NotNull] private readonly IProgress<ProgressData> progressFixCsProjectFiles;

        public MainWindowViewModel(
            [NotNull] IStatusViewModel statusViewModel,
            [NotNull] IProjectCollectionViewModel projectCollectionViewModel,
            [NotNull] IConfigurationService configurationService,
            [NotNull] IConfiguration configuration,
            [NotNull] IEntityEditor showInDialog)
        {
            Guard.NotNull(configurationService, nameof(configurationService));
            Guard.NotNull(configuration, nameof(configuration));
            Guard.NotNull(showInDialog, nameof(showInDialog));
            ProjectCollection = Guard.NotNull(projectCollectionViewModel, nameof(projectCollectionViewModel));
            StatusViewModel = Guard.NotNull(statusViewModel, nameof(statusViewModel));

            progressFixCsProjectFiles = new Progress<ProgressData>(data =>
            {
                if (string.IsNullOrEmpty(data.Message))
                    return;

                // THIS IS PROBABLY NOT THE WAY TO DO THIS..
                FixCsProjectFilesLog += data.Message + Environment.NewLine;
            });

            WorkingDirectory = configuration.RootPath ?? string.Empty;

            OpenSettings = new OpenSettingsCommand(showInDialog, configurationService, WorkingDirectory);
            Initialize = new CapturingExceptionAsyncCommand(async () =>
            {
                await Task.WhenAll(
                        ProjectCollection.Initialize.ExecuteAsync(null),
                        StatusViewModel.Initialize.ExecuteAsync(null))
                    .ConfigureAwait(false);
            });
        }

        public IProjectCollectionViewModel ProjectCollection { get; }

        public IStatusViewModel StatusViewModel { get; }

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
            [NotNull] private readonly IEntityEditor entityEditor;
            [NotNull] private readonly IConfigurationService configurationService;
            [NotNull] private readonly string rootPath;

            public OpenSettingsCommand(
                [NotNull] IEntityEditor entityEditor,
                [NotNull] IConfigurationService configurationService,
                [NotNull] string rootPath)
            {
                DebugGuard.NotNull(entityEditor, nameof(entityEditor));
                DebugGuard.NotNull(configurationService, nameof(configurationService));
                DebugGuard.NotNull(rootPath, nameof(rootPath));
                this.entityEditor = entityEditor;
                this.configurationService = configurationService;
                this.rootPath = rootPath;
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

                var result = entityEditor.Edit(applicationSettings);
                if (result.HasValue && result.Value)
                {
                    // todo
                    configurationService.UpdateAsync(applicationSettings);
                }
            }
        }
    }
}
