namespace Treatment.UI.Framework
{
    public interface IEntityEditor
    {
        bool? Edit<TEntity>(TEntity entity)
            where TEntity : class;
    }
}
