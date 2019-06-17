namespace Treatment.UI.Core.Model
{
    using System;
    using Helpers.Guards;
    using JetBrains.Annotations;
    using SimpleInjector;
    using Wpf.Framework.EntityEditor;
    using Wpf.Framework.EntityEditor.View;
    using Wpf.Framework.EntityEditor.ViewModel;

    internal class SimpleInjectorEditorByTypeFactory : IEditorByTypeFactory
    {
        [NotNull] private readonly Container container;

        public SimpleInjectorEditorByTypeFactory([NotNull] Container container)
        {
            Guard.NotNull(container, nameof(container));
            this.container = container;
        }

        public IEntityEditorViewModel<TEntity> GetEditorViewModel<TEntity>(Type editEntityViewModelType)
            where TEntity : class =>
            (IEntityEditorViewModel<TEntity>)container.GetInstance(editEntityViewModelType);

        public IEntityEditorView<TEntity> GetEditorView<TEntity>(Type editEntityViewType)
            where TEntity : class =>
            (IEntityEditorView<TEntity>)container.GetInstance(editEntityViewType);
    }
}
