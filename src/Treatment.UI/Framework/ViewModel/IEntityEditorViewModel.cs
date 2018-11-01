namespace Treatment.UI.Framework.ViewModel
{
    public interface IEntityEditorViewModel<TEntity>
        where TEntity : class
    {
        void Initialize(TEntity entity);
    }
}
