namespace Wpf.Framework.EntityEditor.View
{
    using Wpf.Framework.EntityEditor.ViewModel;

    public interface IEntityEditorView<T>
        where T : class
    {
        void Set(IEntityEditorViewModel<T> viewModel);
    }
}
