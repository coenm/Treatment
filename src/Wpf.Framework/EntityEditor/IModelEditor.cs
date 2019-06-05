namespace Wpf.Framework.EntityEditor
{
    public interface IModelEditor
    {
        bool? Edit<TModel>(TModel entity)
            where TModel : class;
    }
}
