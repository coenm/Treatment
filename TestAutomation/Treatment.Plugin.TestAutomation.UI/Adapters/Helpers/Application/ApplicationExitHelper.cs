namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.Application
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Application;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class ApplicationExitHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly Application application;
        [NotNull] private readonly Action<ApplicationExit> callback;

        public ApplicationExitHelper([NotNull] Application application, [NotNull] Action<ApplicationExit> callback)
        {
            Guard.NotNull(application, nameof(application));
            Guard.NotNull(callback, nameof(callback));

            this.application = application;
            this.callback = callback;
        }

        public void Initialize()
        {
            application.Exit += ApplicationOnExit;
        }

        public void Dispose()
        {
            application.Exit -= ApplicationOnExit;
        }

        private void ApplicationOnExit(object sender, ExitEventArgs e)
        {
            var evt = new ApplicationExit
                      {
                          ApplicationExitCode = e.ApplicationExitCode,
                      };

            callback.Invoke(evt);
        }
    }
}
