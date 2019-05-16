namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.WindowControl
{
    using System;
    using System.ComponentModel;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Window;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class WindowClosingHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly Window window;
        [NotNull] private readonly Action<WindowClosing> callback;

        public WindowClosingHelper([NotNull] Window window, [NotNull] Action<WindowClosing> callback)
        {
            Guard.NotNull(window, nameof(window));
            Guard.NotNull(callback, nameof(callback));

            this.window = window;
            this.callback = callback;
        }

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
            callback.Invoke(new WindowClosing());
        }
    }
}
