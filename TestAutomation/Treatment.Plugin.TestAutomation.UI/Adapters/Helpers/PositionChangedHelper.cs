namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class PositionChangedHelper : IUiElement, IDisposable
    {
        [NotNull] private readonly FrameworkElement frameworkElement;
        [NotNull] private readonly IEventPublisher eventPublisher;
        [CanBeNull] private Window registeredWindow;
        [CanBeNull] private Point position;

        public PositionChangedHelper([NotNull] FrameworkElement frameworkElement, [NotNull] IEventPublisher eventPublisher, [NotNull] Guid guid)
        {
            Guard.NotNull(frameworkElement, nameof(frameworkElement));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.frameworkElement = frameworkElement;
            this.eventPublisher = eventPublisher;
            Guid = guid;
        }

        public Guid Guid { get; }

        public void Initialize()
        {
            // expensive..
            frameworkElement.LayoutUpdated += OnLayoutUpdated;

            // todo inject window to subscribe on?!
            registeredWindow = Application.Current.MainWindow;

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
                var pos = frameworkElement.PointToScreen(new Point(0d, 0d));
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
                          Guid = Guid,
                          Point = position,
                      };
            eventPublisher.PublishAsync(evt);
        }
    }
}
