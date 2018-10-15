namespace Treatment.UI.ViewModel.Settings
{
    using System;

    using JetBrains.Annotations;

    public class ApplicationSettingsViewModel : ViewModelBase, IEntityEditorViewModel<ApplicationSettings>
    {
        [NotNull] private ApplicationSettings _entity;
        private bool _delayExecution;
        private string _searchProviderName;

        public void Initialize(ApplicationSettings entity)
        {
            _entity = entity ?? throw new ArgumentNullException(nameof(entity));

            DelayExecution = entity.DelayExecution;
            SearchProviderName = entity.SearchProviderName;
        }

        public bool DelayExecution
        {
            get => _delayExecution;
            set
            {
                if (_delayExecution == value)
                    return;
                _delayExecution = value;
                OnPropertyChanged();
            }
        }

        public string SearchProviderName
        {
            get => _searchProviderName;
            set
            {
                if (_searchProviderName == value)
                    return;
                _searchProviderName = value;
                OnPropertyChanged();
            }
        }

        public ApplicationSettings Export()
        {
            return new ApplicationSettings
                       {
                           DelayExecution = DelayExecution,
                           SearchProviderName = SearchProviderName
                       };
        }

    }
}