namespace Wpf.Framework.EntityEditor.ViewModel
{
    public interface IEntityEditorViewModel<TEntity>
        where TEntity : class
    {
        void Initialize(TEntity entity);

        void SaveToEntity();
    }
}
