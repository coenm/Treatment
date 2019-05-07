namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.WindowControl
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Window;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class WindowActivatedDeactivatedHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly Window window;
        [NotNull] private readonly IEventPublisher eventPublisher;

        public WindowActivatedDeactivatedHelper([NotNull] Window window, [NotNull] IEventPublisher eventPublisher, [NotNull] Guid guid)
        {
            Guard.NotNull(window, nameof(window));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.window = window;
            this.eventPublisher = eventPublisher;
            Guid = guid;
        }

        public Guid Guid { get; }

        public void Initialize()
        {
            window.Activated += WindowOnActivated;
            window.Deactivated += WindowOnDeactivated;
        }

        public void Dispose()
        {
            window.Activated -= WindowOnActivated;
            window.Deactivated -= WindowOnDeactivated;
        }

        private void WindowOnDeactivated(object sender, EventArgs e)
        {
            var evt = new WindowDeactivated
                      {
                          Guid = Guid,
                      };

            eventPublisher.PublishAsync(evt);
        }

        private void WindowOnActivated(object sender, EventArgs e)
        {
            var evt = new WindowActivated
                      {
                          Guid = Guid,
                      };

            eventPublisher.PublishAsync(evt);
        }
    }
}
