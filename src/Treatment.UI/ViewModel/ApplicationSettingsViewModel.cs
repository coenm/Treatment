namespace Treatment.UI.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Nito.Mvvm;
    using Treatment.Contract;
    using Treatment.Contract.Queries;
    using Treatment.Helpers.Guards;
    using Treatment.UI.Core.Configuration;
    using Treatment.UI.Framework.ViewModel;

    public class ApplicationSettingsViewModel : ViewModelBase, IEntityEditorViewModel<ApplicationSettings>
    {
        [NotNull] private readonly CapturingExceptionAsyncCommand getSearchProvidersCommand;
        [NotNull] private readonly CapturingExceptionAsyncCommand getVersionControlProvidersCommand;
        [CanBeNull] private ApplicationSettings entity;

        [UsedImplicitly]
        public ApplicationSettingsViewModel([NotNull] IQueryProcessor queryProcessor)
        {
            Guard.NotNull(queryProcessor, nameof(queryProcessor));

            SearchProviderNames = new ObservableCollection<string>();
            getSearchProvidersCommand = new CapturingExceptionAsyncCommand(async () =>
            {
                if (entity?.DelayExecution ?? true)
                    await Task.Delay(1000);

                var result = await queryProcessor.ExecuteQueryAsync(GetAllSearchProvidersQuery.Instance);

                var tmp = SearchProviderName;

                SearchProviderNames.Clear();

                foreach (var item in result.OrderBy(x => x.Priority))
                    SearchProviderNames.Add(item.Name);

                if (SearchProviderNames.Contains(tmp))
                {
                    SearchProviderName = tmp;
                    return;
                }

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

                var tmp = VersionControlProviderName;
                VersionControlProviderNames.Clear();

                foreach (var item in result.OrderBy(x => x.Priority))
                    VersionControlProviderNames.Add(item.Name);

                if (VersionControlProviderNames.Contains(tmp))
                {
                    VersionControlProviderName = tmp;
                    return;
                }

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
            Guard.NotNull(applicationSettings, nameof(applicationSettings));
            entity = applicationSettings;

            DelayExecution = applicationSettings.DelayExecution;
            SearchProviderName = applicationSettings.SearchProviderName;
            VersionControlProviderName = applicationSettings.VersionControlProviderName;
            RootDirectory = applicationSettings.RootDirectory;

            ((System.Windows.Input.ICommand)getSearchProvidersCommand).Execute(null);
            ((System.Windows.Input.ICommand)getVersionControlProvidersCommand).Execute(null);
        }

        public void SaveToEntity()
        {
            DebugGuard.NotNull(entity, nameof(entity));
            entity.DelayExecution = DelayExecution;
            entity.SearchProviderName = SearchProviderName;
            entity.RootDirectory = RootDirectory;
            entity.VersionControlProviderName = VersionControlProviderName;
        }
    }
}
