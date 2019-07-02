namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class LoadedUnLoadedHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly FrameworkElement frameworkElement;
        [NotNull] private readonly Action<OnLoaded> onLoadedCallback;
        [NotNull] private readonly Action<OnUnLoaded> onUnLoadedCallback;

        public LoadedUnLoadedHelper(
            [NotNull] FrameworkElement frameworkElement,
            [NotNull] Action<OnLoaded> onLoadedCallback,
            [NotNull] Action<OnUnLoaded> onUnLoadedCallback)
        {
            Guard.NotNull(frameworkElement, nameof(frameworkElement));
            Guard.NotNull(onLoadedCallback, nameof(onLoadedCallback));
            Guard.NotNull(onUnLoadedCallback, nameof(onUnLoadedCallback));

            this.frameworkElement = frameworkElement;
            this.onLoadedCallback = onLoadedCallback;
            this.onUnLoadedCallback = onUnLoadedCallback;
        }

        public void Initialize()
        {
            frameworkElement.Loaded += FrameworkElementOnLoaded;
            frameworkElement.Unloaded += FrameworkElementOnUnloaded;
        }

        public void Dispose()
        {
            frameworkElement.Loaded -= FrameworkElementOnLoaded;
            frameworkElement.Unloaded -= FrameworkElementOnUnloaded;
        }

        private void FrameworkElementOnLoaded(object sender, RoutedEventArgs e)
        {
            onLoadedCallback.Invoke(new OnLoaded());
        }

        private void FrameworkElementOnUnloaded(object sender, RoutedEventArgs e)
        {
            onUnLoadedCallback.Invoke(new OnUnLoaded());
        }
    }
}
