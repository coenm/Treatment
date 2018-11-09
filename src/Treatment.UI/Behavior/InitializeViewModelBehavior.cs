namespace Treatment.UI.Behavior
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Interactivity;

    using JetBrains.Annotations;
    using Treatment.UI.Framework.ViewModel;
    public class InitializeViewModelBehavior : Behavior<Window>
    {
        private IDisposable registration;

        protected override void OnAttached()
        {
            base.OnAttached();
            var dataContext = AssociatedObject.DataContext;
            if (dataContext == null)
            {
                var initializeOnContextChange = new SingleInitializeExecutionOnDataContext(AssociatedObject);
                initializeOnContextChange.Register();
                registration = initializeOnContextChange;
                return;
            }

            if (!(dataContext is IInitializableViewModel vm))
                return;

            if (vm.Initialize.CanExecute(null))
                vm.Initialize.Execute(null);
        }

        protected override void OnDetaching()
        {
            registration?.Dispose();
            registration = null;
            base.OnDetaching();
        }

        private class SingleInitializeExecutionOnDataContext : IDisposable
        {
            [NotNull] private readonly object registerEventHandlersSyncLock = new object();
            [NotNull] private readonly Window window;
            private bool windowDataContextChangedEventSubscribed;
            private bool windowLoadEventSubscribed;
            private bool windowIsLoaded;

            public SingleInitializeExecutionOnDataContext(Window window)
            {
                this.window = window;
                windowDataContextChangedEventSubscribed = false;
                windowLoadEventSubscribed = false;
                windowIsLoaded = false;
            }

            public void Register()
            {
                lock (registerEventHandlersSyncLock)
                {
                    if (windowDataContextChangedEventSubscribed)
                        return;

                    window.DataContextChanged += WindowOnDataContextChanged;
                    windowIsLoaded = window.IsLoaded;

                    if (!windowLoadEventSubscribed)
                    {
                        window.Loaded += Window_Loaded;
                        windowLoadEventSubscribed = true;
                    }

                    windowDataContextChangedEventSubscribed = true;
                }
            }

            public void Dispose() => Unregister();

            private void WindowOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                ExecuteInitialize();
            }

            private void Window_Loaded(object sender, RoutedEventArgs e)
            {
                windowIsLoaded = true;

                lock (registerEventHandlersSyncLock)
                {
                    if (windowLoadEventSubscribed)
                    {
                        window.Loaded -= Window_Loaded;
                        windowLoadEventSubscribed = false;
                    }
                }

                ExecuteInitialize();
            }

            private void ExecuteInitialize()
            {
                if (!windowDataContextChangedEventSubscribed)
                    return;

                if (!windowIsLoaded || !(window.DataContext is IInitializableViewModel vm))
                    return;

                if (vm.Initialize.CanExecute(null))
                    vm.Initialize.Execute(null);

                Unregister();
            }

            private void Unregister()
            {
                lock (registerEventHandlersSyncLock)
                {
                    if (!windowDataContextChangedEventSubscribed)
                        return;

                    if (windowLoadEventSubscribed)
                    {
                        window.Loaded -= Window_Loaded;
                        windowLoadEventSubscribed = false;
                    }

                    window.DataContextChanged -= WindowOnDataContextChanged;
                    windowDataContextChangedEventSubscribed = false;
                }
            }
        }
    }
}
