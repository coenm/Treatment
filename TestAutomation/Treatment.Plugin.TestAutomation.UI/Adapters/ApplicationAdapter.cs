namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Application;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Treatment;

    public class ApplicationAdapter : IApplication
    {
        [NotNull] private readonly Application item;
        [NotNull] private readonly IEventPublisher eventPublisher;

        public ApplicationAdapter(
            [NotNull] Application item,
            [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.item = item;
            this.eventPublisher = eventPublisher;

            Guid = Guid.NewGuid();

            eventPublisher.PublishAsync(new ApplicationStarting
            {
                CountDown = 0,
            });
        }

        public Guid Guid { get; }

        public IMainView MainView { get; private set; }

        public void Initialize()
        {
            item.Startup += ItemOnStartup;
            item.Activated += ItemOnActivated;
            item.Deactivated += ItemOnDeactivated;
            item.Exit += ItemOnExit;
            item.DispatcherUnhandledException += ItemOnDispatcherUnhandledException;
        }

        public void RegisterAndInitializeMainView(IMainView mainView)
        {
            Guard.NotNull(mainView, nameof(mainView));
            MainView = mainView;
            mainView.Initialize();
        }

        public void Dispose()
        {
            item.Startup -= ItemOnStartup;
            item.Activated -= ItemOnActivated;
            item.Deactivated -= ItemOnDeactivated;
            item.Exit -= ItemOnExit;
            item.DispatcherUnhandledException -= ItemOnDispatcherUnhandledException;
        }

        private void ItemOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) =>
            eventPublisher.PublishAsync(new ApplicationDispatcherUnhandledException
            {
                Guid = Guid,
            });

        private void ItemOnExit(object sender, ExitEventArgs e)
        {
            eventPublisher.PublishAsync(new ApplicationExit
            {
                Guid = Guid,
                ApplicationExitCode = e.ApplicationExitCode,
            });
        }

        private void ItemOnDeactivated(object sender, EventArgs e) => eventPublisher.PublishAsync(new ApplicationDeactivated
        {
            Guid = Guid,
        });

        private void ItemOnStartup(object sender, StartupEventArgs e) => eventPublisher.PublishAsync(new ApplicationStarted
        {
            Guid = Guid,
        });

        private void ItemOnActivated(object sender, EventArgs e) => eventPublisher.PublishAsync(new ApplicationActivated
        {
            Guid = Guid,
        });
    }
}
