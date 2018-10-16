namespace Treatment.UI.Core.UI
{
    public interface IShowEntityInDialogProcessor
    {
        bool? ShowDialog<TEntity>(TEntity entity) where TEntity : class;
    }
}