namespace Treatment.UI.Framework
{
    public interface IShowEntityInDialogProcessor
    {
        bool? ShowDialog<TEntity>(TEntity entity)
            where TEntity : class;
    }
}
