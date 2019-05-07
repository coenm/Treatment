namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.Application
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Application;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class ApplicationActivationHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly Application application;
        [NotNull] private readonly IEventPublisher eventPublisher;

        public ApplicationActivationHelper([NotNull] Application application, [NotNull] IEventPublisher eventPublisher, [NotNull] Guid guid)
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
            var evt = new ApplicationDeactivated
                      {
                          Guid = Guid,
                      };

            eventPublisher.PublishAsync(evt);
        }

        private void ApplicationOnActivated(object sender, EventArgs e)
        {
            var evt = new ApplicationActivated
                      {
                          Guid = Guid,
                      };

            eventPublisher.PublishAsync(evt);
        }
    }
}
