namespace Treatment.UI.Core.ViewModel
{
    using System;
    using System.Threading.Tasks;
    using Contract;
    using Core.Configuration;
    using Treatment.Helpers.Guards;
    using JetBrains.Annotations;
    using Nito.Mvvm;
    using NLog;
    using Wpf.Framework.EntityEditor;
    using Wpf.Framework.ViewModel;
    using ICommand = System.Windows.Input.ICommand;

    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel, IInitializableViewModel
    {
        [NotNull] private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public MainWindowViewModel(
            [NotNull] IStatusViewModel statusViewModel,
            [NotNull] IProjectCollectionViewModel projectCollectionViewModel,
            [NotNull] IConfigurationService configurationService,
            [NotNull] IModelEditor showInDialog)
        {
            Guard.NotNull(configurationService, nameof(configurationService));
            Guard.NotNull(showInDialog, nameof(showInDialog));
            Guard.NotNull(projectCollectionViewModel, nameof(projectCollectionViewModel));
            Guard.NotNull(statusViewModel, nameof(statusViewModel));

            ProjectCollection = projectCollectionViewModel;
            StatusViewModel = statusViewModel;

            WorkingDirectory = string.Empty;
            Logger.Info("ctor");

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
            [NotNull] private readonly IModelEditor modelEditor;
            [NotNull] private readonly IConfigurationService configurationService;
            [NotNull] private readonly string rootPath;

            public OpenSettingsCommand(
                [NotNull] IModelEditor modelEditor,
                [NotNull] IConfigurationService configurationService,
                [NotNull] string rootPath)
            {
                DebugGuard.NotNull(modelEditor, nameof(modelEditor));
                DebugGuard.NotNull(configurationService, nameof(configurationService));
                DebugGuard.NotNull(rootPath, nameof(rootPath));
                this.modelEditor = modelEditor;
                this.configurationService = configurationService;
                this.rootPath = rootPath;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter) => true;

            // todo
            public void Execute(object parameter)
            {
                var applicationSettings = configurationService.GetAsync().GetAwaiter().GetResult();

                var result = modelEditor.Edit(applicationSettings);
                if (result.HasValue && result.Value)
                {
                    configurationService.UpdateAsync(applicationSettings);
                }
            }
        }
    }
}
