namespace Treatment.UI.ViewModel
{
    using System.Collections.ObjectModel;

    public interface IMainWindowViewModel
    {
        ObservableCollection<ProjectViewModel> Sources { get; }
    }


    /// <remarks>
    /// Only to be used as a DesignInstance in xaml.
    /// </remarks>>
    public abstract class DummyMainWindowViewModel : IMainWindowViewModel
    {
        public abstract ObservableCollection<ProjectViewModel> Sources { get; }
    }
}