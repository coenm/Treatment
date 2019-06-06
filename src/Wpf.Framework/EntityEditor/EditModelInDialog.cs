namespace Wpf.Framework.EntityEditor
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Wpf.Framework.Application;
    using Wpf.Framework.EntityEditor.View;
    using Wpf.Framework.EntityEditor.ViewModel;

    public class EditModelInDialog : IModelEditor
    {
        [NotNull] private readonly IGetActivatedWindow getActivatedWindow;
        [NotNull] private readonly IEditorByTypeFactory editorFactoryByType;

        public EditModelInDialog(
            [NotNull] IEditorByTypeFactory editorFactoryByType,
            [NotNull] IGetActivatedWindow getActivatedWindow)
        {
            Guard.NotNull(editorFactoryByType, nameof(editorFactoryByType));
            Guard.NotNull(getActivatedWindow, nameof(getActivatedWindow));

            this.editorFactoryByType = editorFactoryByType;
            this.getActivatedWindow = getActivatedWindow;
        }

        /// <summary>Edit the <paramref name="entity"/> using a popup dialog.</summary>
        /// <typeparam name="TEntity">entity type.</typeparam>
        /// <param name="entity">entity to show. Cannot be null.</param>
        /// <returns>A Nullable bool that specifies whether the activity was accepted (<c>true</c>.) or canceled (<c>false</c>).
        /// The return value is the value of the DialogResult property before a window closes.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <remarks>inspired by <see href="https://stackoverflow.com/questions/28295804/simple-injector-inject-multiple-dependency-in-baseclass/28310234#28310234"/>.</remarks>
        public bool? Edit<TEntity>([NotNull] TEntity entity)
            where TEntity : class
        {
            Guard.NotNull(entity, nameof(entity));

            var entityType = entity.GetType();

            // Compose type
            var editEntityViewModelType = typeof(IEntityEditorViewModel<>).MakeGenericType(entityType);

            // Ask factory for the corresponding ViewModel,
            // which is responsible for editing this type of entity
            var editEntityViewModel = editorFactoryByType.GetEditorViewModel<TEntity>(editEntityViewModelType);

            // give the viewmodel the entity to be edited
            editEntityViewModel.Initialize(entity);

            var editEntityViewType = typeof(IEntityEditorView<>).MakeGenericType(entityType);
            var view = editorFactoryByType.GetEditorView<TEntity>(editEntityViewType);

            // give the view the viewmodel
            view.Set(editEntityViewModel);

            if (!(view is Window window))
                return null;

            window.Owner = getActivatedWindow.Current;

            var result = window.ShowDialog();
            if (!result.HasValue || result != true)
                return null;

            editEntityViewModel.SaveToEntity();
            return true;
        }
    }
}
