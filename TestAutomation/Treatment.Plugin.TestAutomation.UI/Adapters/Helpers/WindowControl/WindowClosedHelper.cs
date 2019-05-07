namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.WindowControl
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Window;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class WindowClosedHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly Window window;
        [NotNull] private readonly IEventPublisher eventPublisher;

        public WindowClosedHelper([NotNull] Window window, [NotNull] IEventPublisher eventPublisher, [NotNull] Guid guid)
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
            window.Closed += WindowOnClosed;
        }

        public void Dispose()
        {
            window.Closed -= WindowOnClosed;
        }

        private void WindowOnClosed(object sender, EventArgs e)
        {
            var evt = new WindowClosed
                      {
                          Guid = Guid,
                      };

            eventPublisher.PublishAsync(evt);
        }
    }
}
