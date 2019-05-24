namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.WindowControl
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Window;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class WindowActivatedDeactivatedHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly Window window;
        [NotNull] private readonly Action<WindowActivated> callbackActivated;
        [NotNull] private readonly Action<WindowDeactivated> callbackDeactivated;

        public WindowActivatedDeactivatedHelper(
            [NotNull] Window window,
            [NotNull] Action<WindowActivated> callbackActivated,
            [NotNull] Action<WindowDeactivated> callbackDeactivated)
        {
            Guard.NotNull(window, nameof(window));
            Guard.NotNull(callbackActivated, nameof(callbackActivated));
            Guard.NotNull(callbackDeactivated, nameof(callbackDeactivated));

            this.window = window;
            this.callbackActivated = callbackActivated;
            this.callbackDeactivated = callbackDeactivated;
        }

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
            callbackDeactivated.Invoke(new WindowDeactivated());
        }

        private void WindowOnActivated(object sender, EventArgs e)
        {
            callbackActivated.Invoke(new WindowActivated());
        }
    }
}
