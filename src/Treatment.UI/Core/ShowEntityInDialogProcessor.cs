namespace Treatment.UI.Core
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;

    using SimpleInjector;

    using Treatment.UI.View;
    using Treatment.UI.ViewModel;

    public class ShowEntityInDialogProcessor : IShowEntityInDialogProcessor
    {
        private readonly Container _container;

        public ShowEntityInDialogProcessor([NotNull] Container container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">thrown when <paramref name="entity"/> is null</exception>
        /// <remarks>inspired by https://stackoverflow.com/questions/28295804/simple-injector-inject-multiple-dependency-in-baseclass/28310234#28310234 </remarks>
        public bool? ShowDialog<TEntity>([NotNull] TEntity entity) where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityType = entity.GetType();

            // Compose type
            var editEntityViewModelType = typeof(IEntityEditorViewModel<>).MakeGenericType(entityType);

            // Ask SimpleInjector for the corresponding ViewModel,
            // which is responsible for editing this type of entity
            var editEntityViewModel = (IEntityEditorViewModel<TEntity>)_container.GetInstance(editEntityViewModelType);

            // give the viewmodel the entity to be edited
            editEntityViewModel.Initialize(entity);

            var editEntityViewType = typeof(IEntityEditorView<>).MakeGenericType(entityType);
            var view = (IEntityEditorView<TEntity>)_container.GetInstance(editEntityViewType);

            // give the view the viewmodel
            view.Set(editEntityViewModel);

            if (view is Window window)
                return window.ShowDialog();

            return null;
        }
    }
}