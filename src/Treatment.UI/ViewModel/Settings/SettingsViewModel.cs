namespace Treatment.UI.ViewModel.Settings
{
    public class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private bool _delayExecution;
        private string _searchProviderName;

        public SettingsViewModel()
        {
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
    }
}