namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.Application
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Application;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class ApplicationDispatcherUnhandledExceptionHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly Application application;
        [NotNull] private readonly IEventPublisher eventPublisher;

        public ApplicationDispatcherUnhandledExceptionHelper([NotNull] Application application, [NotNull] IEventPublisher eventPublisher, [NotNull] Guid guid)
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
            application.DispatcherUnhandledException += ApplicationOnDispatcherUnhandledException;
        }

        public void Dispose()
        {
            application.DispatcherUnhandledException -= ApplicationOnDispatcherUnhandledException;
        }

        private void ApplicationOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var evt = new ApplicationDispatcherUnhandledException
                      {
                          Guid = Guid,
                      };

            eventPublisher.PublishAsync(evt);
        }
    }
}
