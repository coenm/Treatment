namespace Wpf.Framework.EntityEditor
{
    using System;

    using JetBrains.Annotations;
    using Wpf.Framework.EntityEditor.View;
    using Wpf.Framework.EntityEditor.ViewModel;

    public interface IEditorByTypeFactory
    {
        IEntityEditorViewModel<TEntity> GetEditorViewModel<TEntity>([NotNull]Type editEntityViewModelType)
            where TEntity : class;

        IEntityEditorView<TEntity> GetEditorView<TEntity>([NotNull] Type type)
            where TEntity : class;
    }
}
