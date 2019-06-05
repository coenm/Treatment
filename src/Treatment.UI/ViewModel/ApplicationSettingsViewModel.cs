namespace Treatment.UI.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Linq;

    using JetBrains.Annotations;
    using Nito.Mvvm;
    using Treatment.Contract;
    using Treatment.Contract.Queries;
    using Treatment.Helpers.Guards;
    using Treatment.UI.Core.Configuration;
    using Treatment.UI.Framework.ViewModel;
    using Treatment.UI.Implementations.Delay;
    using Wpf.Framework.ViewModel;

    public class ApplicationSettingsViewModel : ViewModelBase, IEntityEditorViewModel<ApplicationSettings>
    {
        [NotNull] private readonly CapturingExceptionAsyncCommand getSearchProvidersCommand;
        [NotNull] private readonly CapturingExceptionAsyncCommand getVersionControlProvidersCommand;
        [CanBeNull] private ApplicationSettings entity;

        [UsedImplicitly]
        public ApplicationSettingsViewModel([NotNull] IQueryProcessor queryProcessor, [NotNull] IDelayService delayService)
        {
            Guard.NotNull(queryProcessor, nameof(queryProcessor));
            Guard.NotNull(delayService, nameof(delayService));

            SearchProviderNames = new ObservableCollection<string>();
            getSearchProvidersCommand = new CapturingExceptionAsyncCommand(async () =>
            {
                if (entity?.DelayExecution.Enabled ?? true)
                    await delayService.DelayAsync();

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
                if (entity?.DelayExecution.Enabled ?? true)
                    await delayService.DelayAsync();

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

        public uint DelayMinMilliseconds
        {
            get => Properties.Get<uint>(0);
            set => Properties.Set(value);
        }

        public uint DelayMaxMilliseconds
        {
            get => Properties.Get<uint>(0);
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

            DelayExecution = applicationSettings.DelayExecution.Enabled;
            DelayMinMilliseconds = (uint)applicationSettings.DelayExecution.MinMilliseconds;
            DelayMaxMilliseconds = (uint)applicationSettings.DelayExecution.MaxMilliseconds;
            SearchProviderName = applicationSettings.SearchProviderName;
            VersionControlProviderName = applicationSettings.VersionControlProviderName;
            RootDirectory = applicationSettings.RootDirectory;

            ((System.Windows.Input.ICommand)getSearchProvidersCommand).Execute(null);
            ((System.Windows.Input.ICommand)getVersionControlProvidersCommand).Execute(null);
        }

        public void SaveToEntity()
        {
            DebugGuard.NotNull(entity, nameof(entity));
            entity.DelayExecution.Enabled = DelayExecution;
            entity.DelayExecution.MinMilliseconds = (int)DelayMinMilliseconds;
            entity.DelayExecution.MaxMilliseconds = (int)DelayMaxMilliseconds;
            entity.SearchProviderName = SearchProviderName;
            entity.RootDirectory = RootDirectory;
            entity.VersionControlProviderName = VersionControlProviderName;
        }
    }
}
