namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class PositionChangedHelper : IUiElement, IInitializable, IDisposable
    {
        private static readonly Point ZeroPoint = new Point(0d, 0d);
        [NotNull] private readonly FrameworkElement frameworkElement;
        [NotNull] private readonly Action<PositionUpdated> callback;
        [CanBeNull] private Window registeredWindow;
        private Point position;

        public PositionChangedHelper([NotNull] FrameworkElement frameworkElement, [NotNull] Action<PositionUpdated> callback)
        {
            Guard.NotNull(frameworkElement, nameof(frameworkElement));
            Guard.NotNull(callback, nameof(callback));

            this.frameworkElement = frameworkElement;
            this.callback = callback;
        }

        public void Initialize()
        {
            // expensive..
            frameworkElement.LayoutUpdated += OnLayoutUpdated;

            if (frameworkElement is Window w)
            {
                registeredWindow = w;
            }
            else
            {
                // todo
                registeredWindow = System.Windows.Application.Current.MainWindow;
            }

            if (registeredWindow == null)
                return;

            registeredWindow.LocationChanged += MainWindowOnLocationChanged;
        }

        public void Dispose()
        {
            frameworkElement.LayoutUpdated -= OnLayoutUpdated;

            if (registeredWindow == null)
                return;

            registeredWindow.LocationChanged -= MainWindowOnLocationChanged;
        }

        private void OnLayoutUpdated(object sender, EventArgs e) => PublishPositionIfUpdated();

        private void MainWindowOnLocationChanged(object sender, EventArgs e) => PublishPositionIfUpdated();

        private bool IsPositionUpdated()
        {
            try
            {
                if (PresentationSource.FromVisual(frameworkElement) == null)
                    return false;

                var pos = frameworkElement.PointToScreen(ZeroPoint);
                if (pos == position)
                    return false;

                position = pos;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void PublishPositionIfUpdated()
        {
            if (!IsPositionUpdated())
                return;

            var evt = new PositionUpdated
                      {
                          Point = position,
                      };
            callback.Invoke(evt);
        }
    }
}
