namespace Treatment.UI.Core
{
    public interface IShowEntityInDialogProcessor
    {
        bool? ShowDialog<TEntity>(TEntity entity) where TEntity : class;
    }
}