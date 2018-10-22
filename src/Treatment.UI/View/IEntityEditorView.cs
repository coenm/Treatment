namespace Treatment.UI.View
{
    using Treatment.UI.ViewModel;

    public interface IEntityEditorView<T> 
        where T : class
    {
        void Set(IEntityEditorViewModel<T> viewModel);
    }
}