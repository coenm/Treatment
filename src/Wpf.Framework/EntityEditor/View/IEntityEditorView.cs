namespace Wpf.Framework.EntityEditor.View
{
    using ViewModel;

    public interface IEntityEditorView<T>
        where T : class
    {
        void Set(IEntityEditorViewModel<T> viewModel);
    }
}
