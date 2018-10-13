namespace Treatment.UI.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    public interface IMainWindowViewModel
    {
        ObservableCollection<ProjectViewModel> Sources { get; }

        ICommand OpenSettings { get; }
    }


    /// <remarks>
    /// Only to be used as a DesignInstance in xaml.
    /// </remarks>>
    public abstract class DummyMainWindowViewModel : IMainWindowViewModel
    {
        public abstract ObservableCollection<ProjectViewModel> Sources { get; }
        public abstract ICommand OpenSettings { get; }
    }
}