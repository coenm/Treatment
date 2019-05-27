namespace Treatment.UI.Framework
{
    public interface IModelEditor
    {
        bool? Edit<TModel>(TModel entity)
            where TModel : class;
    }
}
