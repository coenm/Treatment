namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.Application
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Application;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class ApplicationActivationHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly Application application;
        [NotNull] private readonly Action<ApplicationActivated> callbackActivated;
        [NotNull] private readonly Action<ApplicationDeactivated> callbackDeactivated;

        public ApplicationActivationHelper(
            [NotNull] Application application,
            [NotNull] Action<ApplicationActivated> callbackActivated,
            [NotNull] Action<ApplicationDeactivated> callbackDeactivated)
        {
            Guard.NotNull(application, nameof(application));
            Guard.NotNull(callbackActivated, nameof(callbackActivated));
            Guard.NotNull(callbackDeactivated, nameof(callbackDeactivated));

            this.application = application;
            this.callbackActivated = callbackActivated;
            this.callbackDeactivated = callbackDeactivated;
        }

        public void Initialize()
        {
            application.Activated += ApplicationOnActivated;
            application.Deactivated += ApplicationOnDeactivated;
        }

        public void Dispose()
        {
            application.Activated -= ApplicationOnActivated;
            application.Deactivated -= ApplicationOnDeactivated;
        }

        private void ApplicationOnDeactivated(object sender, EventArgs e)
        {
            var evt = new ApplicationDeactivated();
            callbackDeactivated.Invoke(evt);
        }

        private void ApplicationOnActivated(object sender, EventArgs e)
        {
            var evt = new ApplicationActivated();
            callbackActivated.Invoke(evt);
        }
    }
}
