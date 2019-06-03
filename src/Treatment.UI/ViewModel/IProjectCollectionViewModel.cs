namespace Treatment.UI.ViewModel
{
    using System.Collections.ObjectModel;

    using Wpf.Framework.Commands.Nito;

    public interface IProjectCollectionViewModel
    {
        ObservableCollection<ProjectViewModel> Projects { get; }

        CapturingExceptionAsyncCommand Initialize { get; }
    }
}
