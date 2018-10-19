﻿namespace Treatment.UI.ViewModel.Settings
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Nito.Mvvm;

    using Treatment.Contract;
    using Treatment.Contract.Queries;

    public class ApplicationSettingsViewModel : ViewModelBase, IEntityEditorViewModel<ApplicationSettings>
    {
        [NotNull] private readonly CapturingExceptionAsyncCommand _getSearchProvidersCommand;
        [NotNull] private readonly CapturingExceptionAsyncCommand _getVersionControlProvidersCommand;
        [CanBeNull] private ApplicationSettings _entity;

        [UsedImplicitly]
        public ApplicationSettingsViewModel([NotNull] IQueryProcessor queryProcessor)
        {
            if (queryProcessor == null)
                throw new ArgumentNullException(nameof(queryProcessor));

            SearchProviderNames = new ObservableCollection<string>();
            _getSearchProvidersCommand = new CapturingExceptionAsyncCommand(async () =>
                                                                            {
                                                                                if (_entity?.DelayExecution ?? true)
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
            _getVersionControlProvidersCommand = new CapturingExceptionAsyncCommand(async () =>
                                                                                    {
                                                                                        if (_entity?.DelayExecution ?? true)
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

        public NotifyTask GetSearchProvidersTask => _getSearchProvidersCommand.Execution;
        public NotifyTask GetVersionControlProvidersTask => _getVersionControlProvidersCommand.Execution;

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

        public void Initialize(ApplicationSettings entity)
        {
            _entity = entity ?? throw new ArgumentNullException(nameof(entity));

            DelayExecution = entity.DelayExecution;
            SearchProviderName = entity.SearchProviderName;
            VersionControlProviderName = entity.VersionControlProviderName;
            RootDirectory = entity.RootDirectory;

            ((System.Windows.Input.ICommand)_getSearchProvidersCommand).Execute(null);
            ((System.Windows.Input.ICommand)_getVersionControlProvidersCommand).Execute(null);
        }

        public ApplicationSettings Export()
        {
            return new ApplicationSettings
                       {
                           DelayExecution = DelayExecution,
                           SearchProviderName = SearchProviderName,
                           RootDirectory = RootDirectory
                       };
        }
    }
}