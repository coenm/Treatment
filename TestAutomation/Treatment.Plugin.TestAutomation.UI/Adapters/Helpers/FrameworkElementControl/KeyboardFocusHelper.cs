namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class KeyboardFocusHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly FrameworkElement frameworkElement;
        [NotNull] private readonly Action<KeyboardFocusChanged> callback;

        public KeyboardFocusHelper(
            [NotNull] FrameworkElement frameworkElement,
            [NotNull] Action<KeyboardFocusChanged> callback)
        {
            Guard.NotNull(frameworkElement, nameof(frameworkElement));
            Guard.NotNull(callback, nameof(callback));

            this.frameworkElement = frameworkElement;
            this.callback = callback;
        }

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
                          Focussed = false,
                      };

            callback.Invoke(evt);
        }

        private void ItemOnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var evt = new KeyboardFocusChanged
                      {
                          Focussed = true,
                      };

            callback.Invoke(evt);
        }
    }
}
