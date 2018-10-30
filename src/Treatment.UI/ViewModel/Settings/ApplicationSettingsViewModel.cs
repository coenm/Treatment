namespace Treatment.UI.ViewModel.Settings
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Nito.Mvvm;
    using Treatment.Contract;
    using Treatment.Contract.Queries;
    using Treatment.UI.Core.Configuration;

    public class ApplicationSettingsViewModel : ViewModelBase, IEntityEditorViewModel<ApplicationSettings>
    {
        [NotNull]
        private readonly CapturingExceptionAsyncCommand getSearchProvidersCommand;

        [NotNull]
        private readonly CapturingExceptionAsyncCommand getVersionControlProvidersCommand;

        [CanBeNull]
        private ApplicationSettings entity;

        [UsedImplicitly]
        public ApplicationSettingsViewModel([NotNull] IQueryProcessor queryProcessor)
        {
            if (queryProcessor == null)
                throw new ArgumentNullException(nameof(queryProcessor));

            SearchProviderNames = new ObservableCollection<string>();
            getSearchProvidersCommand = new CapturingExceptionAsyncCommand(async () =>
            {
                if (entity?.DelayExecution ?? true)
                    await Task.Delay(1000);

                var result = await queryProcessor.ExecuteQueryAsync(GetAllSearchProvidersQuery.Instance);
                foreach (var item in result.OrderBy(x => x.Priority))
                    SearchProviderNames.Add(item.Name);

                if (SearchProviderNames.Contains(SearchProviderName))
                    return;

                SearchProviderName = result
                    .OrderBy(x => x.Priority)
                    .FirstOrDefault()
                    ?.Name;
            });

            VersionControlProviderNames = new ObservableCollection<string>();
            getVersionControlProvidersCommand = new CapturingExceptionAsyncCommand(async () =>
            {
                if (entity?.DelayExecution ?? true)
                    await Task.Delay(800);

                var result = await queryProcessor.ExecuteQueryAsync(GetAllVersionControlProvidersQuery.Instance);
                foreach (var item in result.OrderBy(x => x.Priority))
                    VersionControlProviderNames.Add(item.Name);

                if (VersionControlProviderNames.Contains(VersionControlProviderName))
                    return;

                VersionControlProviderName = result
                    .OrderBy(x => x.Priority)
                    .FirstOrDefault()
                    ?.Name;
            });
        }

        public NotifyTask GetSearchProvidersTask => getSearchProvidersCommand.Execution;

        public NotifyTask GetVersionControlProvidersTask => getVersionControlProvidersCommand.Execution;

        public bool DelayExecution
        {
            get => Properties.Get(false);
            set => Properties.Set(value);
        }

        public string RootDirectory
        {
            get => Properties.Get(string.Empty);
            set => Properties.Set(value);
        }

        public string SearchProviderName
        {
            get => Properties.Get(string.Empty);
            set => Properties.Set(value);
        }

        public ObservableCollection<string> SearchProviderNames { get; }

        public string VersionControlProviderName
        {
            get => Properties.Get(string.Empty);
            set => Properties.Set(value);
        }

        public ObservableCollection<string> VersionControlProviderNames { get; }

        public void Initialize(ApplicationSettings applicationSettings)
        {
            entity = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));

            DelayExecution = applicationSettings.DelayExecution;
            SearchProviderName = applicationSettings.SearchProviderName;
            VersionControlProviderName = applicationSettings.VersionControlProviderName;
            RootDirectory = applicationSettings.RootDirectory;

            ((System.Windows.Input.ICommand)getSearchProvidersCommand).Execute(null);
            ((System.Windows.Input.ICommand)getVersionControlProvidersCommand).Execute(null);
        }

        public ApplicationSettings Export()
        {
            return new ApplicationSettings
                       {
                           DelayExecution = DelayExecution,
                           SearchProviderName = SearchProviderName,
                           RootDirectory = RootDirectory,
                       };
        }
    }
}
