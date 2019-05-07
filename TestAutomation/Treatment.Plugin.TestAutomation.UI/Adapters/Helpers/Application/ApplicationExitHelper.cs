namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.Application
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Application;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class ApplicationExitHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly Application application;
        [NotNull] private readonly IEventPublisher eventPublisher;

        public ApplicationExitHelper([NotNull] Application application, [NotNull] IEventPublisher eventPublisher, [NotNull] Guid guid)
        {
            Guard.NotNull(application, nameof(application));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.application = application;
            this.eventPublisher = eventPublisher;
            Guid = guid;
        }

        public Guid Guid { get; }

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
                          Guid = Guid,
                          ApplicationExitCode = e.ApplicationExitCode,
                      };

            eventPublisher.PublishAsync(evt);
        }
    }
}
