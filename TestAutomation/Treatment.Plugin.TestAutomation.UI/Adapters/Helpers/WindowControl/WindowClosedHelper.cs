namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.WindowControl
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Window;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class WindowClosedHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly Window window;
        [NotNull] private readonly Action<WindowClosed> callback;

        public WindowClosedHelper([NotNull] Window window, [NotNull] Action<WindowClosed> callback)
        {
            Guard.NotNull(window, nameof(window));
            Guard.NotNull(callback, nameof(callback));

            this.window = window;
            this.callback = callback;
        }

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
            callback.Invoke(new WindowClosed());
        }
    }
}
