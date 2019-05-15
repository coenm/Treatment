namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class SizeChangedHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly FrameworkElement frameworkElement;
        [NotNull] private readonly Action<SizeUpdated> callback;

        public SizeChangedHelper([NotNull] FrameworkElement frameworkElement, [NotNull] Action<SizeUpdated> callback)
        {
            Guard.NotNull(frameworkElement, nameof(frameworkElement));
            Guard.NotNull(callback, nameof(callback));

            this.frameworkElement = frameworkElement;
            this.callback = callback;
        }

        public void Initialize()
        {
            frameworkElement.SizeChanged += ItemOnSizeChanged;
        }

        public void Dispose()
        {
            frameworkElement.SizeChanged -= ItemOnSizeChanged;
            PublishEvent(new Size(frameworkElement.ActualWidth, frameworkElement.ActualHeight));
        }

        private void ItemOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            PublishEvent(e.NewSize);
        }

        private void PublishEvent(Size size)
        {
            callback.Invoke(new SizeUpdated
            {
                Size = size,
            });
        }
    }
}
