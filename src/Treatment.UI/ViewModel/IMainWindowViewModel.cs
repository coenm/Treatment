namespace Treatment.UI.ViewModel
{
    using System.Windows.Input;

    public interface IMainWindowViewModel
    {
        IProjectCollectionViewModel ProjectCollection { get; }

        ICommand OpenSettings { get; }

        IStatusViewModel StatusViewModel { get; }
    }
}
