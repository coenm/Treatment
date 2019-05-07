namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class KeyboardFocusHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly FrameworkElement frameworkElement;
        [NotNull] private readonly IEventPublisher eventPublisher;

        public KeyboardFocusHelper([NotNull] FrameworkElement frameworkElement, [NotNull] IEventPublisher eventPublisher, [NotNull] Guid guid)
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
            frameworkElement.GotKeyboardFocus += ItemOnGotKeyboardFocus;
            frameworkElement.LostKeyboardFocus += ItemOnLostKeyboardFocus;
        }

        public void Dispose()
        {
            frameworkElement.GotKeyboardFocus -= ItemOnGotKeyboardFocus;
            frameworkElement.LostKeyboardFocus -= ItemOnLostKeyboardFocus;
        }

        private void ItemOnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var evt = new KeyboardFocusChanged
                      {
                          Guid = Guid,
                          Focussed = false,
                      };

            eventPublisher.PublishAsync(evt);
        }

        private void ItemOnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var evt = new KeyboardFocusChanged
                      {
                          Guid = Guid,
                          Focussed = true,
                      };

            eventPublisher.PublishAsync(evt);
        }
    }
}
