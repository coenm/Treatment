namespace Treatment.UI.ViewModel
{
    using System.Collections.ObjectModel;

    using Nito.Mvvm;

    public interface IProjectCollectionViewModel
    {
        ObservableCollection<ProjectViewModel> Projects { get; }

        CapturingExceptionAsyncCommand Initialize { get; }
    }
}
