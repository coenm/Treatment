namespace Treatment.TestAutomation.TestRunner.Framework.RemoteImplementations
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Application;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Window;
    using Treatment.TestAutomation.Contract.Interfaces.Treatment;
    using Treatment.TestAutomation.TestRunner.Controls.Framework;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;

    internal class RemoteTreatmentApplication : ITreatmentApplication, IDisposable
    {
        [NotNull] private readonly IApplicationEvents applicationEvents;
        [NotNull] private readonly RemoteObjectManager remoteObjectManager;
        [NotNull] private readonly CompositeDisposable disposable;
        [NotNull] private readonly IObservable<IEvent> filter;
        [NotNull] private SingleClassObjectManager propertyManager;
        private Guid guid = Guid.Empty;

        public RemoteTreatmentApplication(
            Guid guid,
            [NotNull] IApplicationEvents applicationEvents,
            [NotNull] RemoteObjectManager remoteObjectManager)
        {
            Guard.NotNull(applicationEvents, nameof(applicationEvents));
            Guard.NotNull(remoteObjectManager, nameof(remoteObjectManager));

            this.applicationEvents = applicationEvents;
            this.remoteObjectManager = remoteObjectManager;

            filter = applicationEvents.Events.Where(x => x.Guid == guid);

            propertyManager = new SingleClassObjectManager(remoteObjectManager, filter);

            State = ApplicationActivationState.Unknown;

            disposable = new CompositeDisposable
            {
                applicationEvents.Events
                    .Where(x => x is ApplicationStarted)
                    .Subscribe(ev =>
                    {
                        Created = true;
                        Startup?.Invoke(this, (ApplicationStarted)ev);
                    }),

                applicationEvents.Events
                    .Where(x => x is ApplicationActivated)
                    .Subscribe(ev =>
                    {
                        State = ApplicationActivationState.Activated;
                        Activated?.Invoke(this, (ApplicationActivated)ev);
                    }),

                applicationEvents.Events
                    .Where(x => x is ApplicationDeactivated)
                    .Subscribe(ev =>
                    {
                        State = ApplicationActivationState.Deactivated;
                        Deactivated?.Invoke(this, (ApplicationDeactivated)ev);
                    }),

                applicationEvents.Events
                    .Where(x => x is ApplicationExit)
                    .Subscribe(ev => Exit?.Invoke(this, (ApplicationExit)ev)),

                applicationEvents.Events
                    .Where(x => x is WindowActivated) // dont care which one.
                    .Subscribe(ev => WindowActivated?.Invoke(this, (WindowActivated)ev)),
            };
        }

        public event EventHandler<WindowActivated> WindowActivated;

        public event EventHandler<ApplicationActivated> Activated;

        public event EventHandler<ApplicationDeactivated> Deactivated;

        public event EventHandler<ApplicationExit> Exit;

        public event EventHandler<ApplicationStarted> Startup;

        public bool Created { get; private set; }

        public ApplicationActivationState State { get; private set; }

        public IMainWindow MainWindow => propertyManager.GetObject<IMainWindow>();

        public ISettingWindow SettingsWindow => propertyManager.GetObject<ISettingWindow>();

        public void Dispose()
        {
            disposable.Dispose();
            propertyManager.Dispose();
        }
    }
}
