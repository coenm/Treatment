namespace Treatment.UI.ViewModel
{
    using System.Windows.Input;

    public interface IMainWindowViewModel
    {
        ProjectCollectionViewModel ProjectCollection { get; }

        ICommand OpenSettings { get; }
    }

    /// <remarks>
    /// Only to be used as a DesignInstance in xaml.
    /// </remarks>>
    public abstract class DummyMainWindowViewModel : IMainWindowViewModel
    {
        public abstract ICommand OpenSettings { get; }

        public abstract ProjectCollectionViewModel ProjectCollection { get; }
    }
}