namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.WindowControl
{
    using System;
    using System.ComponentModel;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Application;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Window;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class WindowClosingHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly Window window;
        [NotNull] private readonly IEventPublisher eventPublisher;

        public WindowClosingHelper([NotNull] Window window, [NotNull] IEventPublisher eventPublisher, [NotNull] Guid guid)
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
            window.Closing += WindowOnClosing;
        }

        public void Dispose()
        {
            window.Closing -= WindowOnClosing;
        }

        private void WindowOnClosing(object sender, CancelEventArgs e)
        {
            var evt = new WindowClosing
                      {
                          Guid = Guid,
                      };

            eventPublisher.PublishAsync(evt);
        }

        private void WindowOnActivated(object sender, EventArgs e)
        {
            var evt = new ApplicationActivated
                      {
                          Guid = Guid,
                      };

            eventPublisher.PublishAsync(evt);
        }
    }
}
