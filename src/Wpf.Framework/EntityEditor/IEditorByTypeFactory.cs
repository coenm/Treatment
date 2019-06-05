namespace Wpf.Framework.EntityEditor
{
    using System;
    using JetBrains.Annotations;
    using View;
    using ViewModel;

    public interface IEditorByTypeFactory
    {
        IEntityEditorViewModel<TEntity> GetEditorViewModel<TEntity>([NotNull]Type editEntityViewModelType)
            where TEntity : class;

        IEntityEditorView<TEntity> GetEditorView<TEntity>([NotNull] Type type)
            where TEntity : class;
    }
}
