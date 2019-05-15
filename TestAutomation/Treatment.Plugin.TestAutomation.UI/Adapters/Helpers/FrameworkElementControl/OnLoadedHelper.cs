namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class OnLoadedHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly FrameworkElement frameworkElement;
        [NotNull] private readonly Action<Loaded> callback;

        public OnLoadedHelper([NotNull] FrameworkElement frameworkElement, [NotNull] Action<Loaded> callback)
        {
            Guard.NotNull(frameworkElement, nameof(frameworkElement));
            Guard.NotNull(this.callback, nameof(this.callback));

            this.frameworkElement = frameworkElement;
            this.callback = callback;
        }

        public void Initialize()
        {
            frameworkElement.Loaded += Loaded;
        }

        public void Dispose()
        {
            frameworkElement.Loaded -= Loaded;
        }

        private void Loaded(object sender, RoutedEventArgs e)
        {
            callback.Invoke(new Loaded());
        }
    }
}
