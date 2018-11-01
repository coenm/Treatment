namespace Treatment.UI.Framework
{
    public interface IEntityEditorViewModel<TEntity>
        where TEntity : class
    {
        void Initialize(TEntity entity);
    }
}
