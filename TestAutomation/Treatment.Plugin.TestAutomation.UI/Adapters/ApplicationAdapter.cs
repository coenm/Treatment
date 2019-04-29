namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;
    using System.Threading;
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

            // this is a special case , otherwise, no publishing etc in ctor.
            for (var countDown = 3; countDown >= 0; countDown--)
            {
                eventPublisher.PublishAsync(new ApplicationStarting
                {
                    CountDown = countDown,
                });

                if (countDown > 0)
                    Thread.Sleep(1000);
            }
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
            eventPublisher.PublishAsync(new ApplicationDispatcherUnhandledException());

        private void ItemOnExit(object sender, ExitEventArgs e)
        {
            eventPublisher.PublishAsync(new ApplicationExit
            {
                ApplicationExitCode = e.ApplicationExitCode,
            });
        }

        private void ItemOnDeactivated(object sender, EventArgs e) => eventPublisher.PublishAsync(new ApplicationDeactivated());

        private void ItemOnStartup(object sender, StartupEventArgs e) => eventPublisher.PublishAsync(new ApplicationStarted());

        private void ItemOnActivated(object sender, EventArgs e) => eventPublisher.PublishAsync(new ApplicationActivated());
    }
}
