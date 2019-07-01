namespace Treatment.TestAutomation.TestRunner.Framework.RemoteImplementations
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    using Contract.Interfaces.Framework;
    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Application;
    using Treatment.TestAutomation.Contract.Interfaces.Events;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Application;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Window;
    using Treatment.TestAutomation.TestRunner.Controls.Framework;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;

    internal class RemoteApplicationImplementation : IApplication, IDisposable
    {
        [NotNull] private readonly IApplicationEvents applicationEvents;
        [NotNull] private readonly RemoteObjectManager remoteObjectManager;
        [NotNull] private readonly CompositeDisposable disposable;
        [NotNull] private readonly SingleClassObjectManager propertyManager;
        private Guid guid = Guid.Empty;

        public RemoteApplicationImplementation(
            Guid guid,
            [NotNull] IApplicationEvents applicationEvents,
            [NotNull] RemoteObjectManager remoteObjectManager)
        {
            Guard.NotNull(applicationEvents, nameof(applicationEvents));
            Guard.NotNull(remoteObjectManager, nameof(remoteObjectManager));

            this.applicationEvents = applicationEvents;
            this.remoteObjectManager = remoteObjectManager;

            var filter = applicationEvents.Events.Where(x => x.Guid == guid);

            propertyManager = new SingleClassObjectManager(remoteObjectManager, filter);

            disposable = new CompositeDisposable
            {
                applicationEvents.Events
                    .Where(x => x is ApplicationStarted)
                    .Subscribe(ev =>
                    {
                        Startup?.Invoke(this, (ApplicationStarted)ev);
                    }),

                applicationEvents.Events
                    .Where(x => x is ApplicationActivated)
                    .Subscribe(ev =>
                    {
                        Activated?.Invoke(this, (ApplicationActivated)ev);
                    }),

                applicationEvents.Events
                    .Where(x => x is ApplicationDeactivated)
                    .Subscribe(ev =>
                    {
                        Deactivated?.Invoke(this, (ApplicationDeactivated)ev);
                    }),

                applicationEvents.Events
                    .Where(x => x is ApplicationExit)
                    .Subscribe(ev => Exit?.Invoke(this, (ApplicationExit)ev)),
            };
        }

        public event EventHandler<ApplicationActivated> Activated;

        public event EventHandler<ApplicationDeactivated> Deactivated;

        public event EventHandler<ApplicationExit> Exit;

        public event EventHandler<ApplicationStarted> Startup;

        public IMainWindow MainWindow => propertyManager.GetObject<IMainWindow>();

        public ISettingWindow SettingsWindow => propertyManager.GetObject<ISettingWindow>();

        public void Dispose()
        {
            disposable.Dispose();
            propertyManager.Dispose();
        }
    }

    internal class ApplicationAdapter : ITreatmentApplication, IDisposable
    {
        [NotNull] private readonly IApplication application;
        [NotNull] private readonly IDisposable disposable;

        public ApplicationAdapter(
            [NotNull] IApplication application,
            [NotNull] IApplicationEvents applicationEvents)
        {
            Guard.NotNull(application, nameof(application));
            Guard.NotNull(applicationEvents, nameof(applicationEvents));

            this.application = application;

            State = ApplicationActivationState.Unknown;

            application.Startup += ApplicationOnStartup;
            application.Activated += ApplicationOnActivated;
            application.Deactivated += ApplicationOnDeactivated;
            application.Exit += ApplicationOnExit;

            State = ApplicationActivationState.Unknown;

            disposable = new CompositeDisposable
            {
                applicationEvents.Events
                    .Where(x => x is WindowActivated) // dont care which one.
                    .Subscribe(ev => WindowActivated?.Invoke(this, (WindowActivated)ev)),
            };
        }

        public event EventHandler<WindowActivated> WindowActivated;

        public event EventHandler<ApplicationActivated> Activated
        {
            add => application.Activated += value;
            remove => application.Activated -= value;
        }

        public event EventHandler<ApplicationDeactivated> Deactivated
        {
            add => application.Deactivated += value;
            remove => application.Deactivated -= value;
        }

        public event EventHandler<ApplicationExit> Exit
        {
            add => application.Exit += value;
            remove => application.Exit -= value;
        }

        public event EventHandler<ApplicationStarted> Startup
        {
            add => application.Startup += value;
            remove => application.Startup -= value;
        }

        public bool Created { get; private set; }

        public ApplicationActivationState State { get; private set; }

        public IMainWindow MainWindow => application.MainWindow;

        public ISettingWindow SettingsWindow => application.SettingsWindow;

        public void Dispose()
        {
            application.Startup -= ApplicationOnStartup;
            application.Activated -= ApplicationOnActivated;
            application.Deactivated -= ApplicationOnDeactivated;
            application.Exit -= ApplicationOnExit;

            disposable.Dispose();
        }

        private void ApplicationOnExit(object sender, ApplicationExit e)
        {
        }

        private void ApplicationOnDeactivated(object sender, ApplicationDeactivated e)
        {
            State = ApplicationActivationState.Deactivated;
        }

        private void ApplicationOnActivated(object sender, ApplicationActivated e)
        {
            State = ApplicationActivationState.Activated;
        }

        private void ApplicationOnStartup(object sender, ApplicationStarted e)
        {
            Created = true;
        }

    }
}
