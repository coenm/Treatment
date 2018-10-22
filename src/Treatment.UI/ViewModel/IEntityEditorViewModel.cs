namespace Treatment.UI.ViewModel
{
    public interface IEntityEditorViewModel<TEntity>
        where TEntity : class
    {
        void Initialize(TEntity entity);
    }
}