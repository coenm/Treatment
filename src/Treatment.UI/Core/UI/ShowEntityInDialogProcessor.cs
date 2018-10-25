namespace Treatment.UI.Core.UI
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;

    using SimpleInjector;

    using Treatment.UI.View;
    using Treatment.UI.ViewModel;

    public class ShowEntityInDialogProcessor : IShowEntityInDialogProcessor
    {
        private readonly Container container;

        public ShowEntityInDialogProcessor([NotNull] Container container)
        {
            this.container = container ?? throw new ArgumentNullException(nameof(container));
        }

        /// <summary>Show dialog for given <paramref name="entity"/>.</summary>
        /// <typeparam name="TEntity">entity type.</typeparam>
        /// <param name="entity">entity to show. Cannot be null.</param>
        /// <returns>A Nullable bool that specifies whether the activity was accepted (<c>true</c>.) or canceled (<c>false</c>).
        /// The return value is the value of the DialogResult property before a window closes.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <remarks>inspired by <see href="https://stackoverflow.com/questions/28295804/simple-injector-inject-multiple-dependency-in-baseclass/28310234#28310234"/>.</remarks>
        public bool? ShowDialog<TEntity>([NotNull] TEntity entity)
            where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityType = entity.GetType();

            // Compose type
            var editEntityViewModelType = typeof(IEntityEditorViewModel<>).MakeGenericType(entityType);

            // Ask SimpleInjector for the corresponding ViewModel,
            // which is responsible for editing this type of entity
            var editEntityViewModel = (IEntityEditorViewModel<TEntity>)container.GetInstance(editEntityViewModelType);

            // give the viewmodel the entity to be edited
            editEntityViewModel.Initialize(entity);

            var editEntityViewType = typeof(IEntityEditorView<>).MakeGenericType(entityType);
            var view = (IEntityEditorView<TEntity>)container.GetInstance(editEntityViewType);

            // give the view the viewmodel
            view.Set(editEntityViewModel);

            if (view is Window window)
                return window.ShowDialog();

            return null;
        }
    }
}
