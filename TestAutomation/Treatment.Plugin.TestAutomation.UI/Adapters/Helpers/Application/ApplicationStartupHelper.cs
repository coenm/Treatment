namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.Application
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Application;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class ApplicationStartupHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly Application application;
        [NotNull] private readonly Action<ApplicationStarted> callback;

        public ApplicationStartupHelper([NotNull] Application application, [NotNull] Action<ApplicationStarted> callback)
        {
            Guard.NotNull(application, nameof(application));
            Guard.NotNull(callback, nameof(callback));

            this.application = application;
            this.callback = callback;
        }

        public void Initialize()
        {
            application.Startup += ApplicationOnStartup;
        }

        public void Dispose()
        {
            application.Startup -= ApplicationOnStartup;
        }

        private void ApplicationOnStartup(object sender, StartupEventArgs e)
        {
            var evt = new ApplicationStarted();
            callback.Invoke(evt);
        }
    }
}
