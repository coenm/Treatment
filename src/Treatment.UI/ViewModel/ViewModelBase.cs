namespace Treatment.UI.ViewModel
{
    using System.ComponentModel;

    using JetBrains.Annotations;

    using Nito.Mvvm.CalculatedProperties;

    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        [NotNull] protected readonly PropertyHelper Properties;

        protected ViewModelBase()
        {
            Properties = new PropertyHelper(RaisePropertyChanged);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }
    }
}