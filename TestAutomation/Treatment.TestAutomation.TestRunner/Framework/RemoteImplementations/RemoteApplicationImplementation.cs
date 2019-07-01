namespace Treatment.TestAutomation.TestRunner.Framework.RemoteImplementations
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Application;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Application;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.TestRunner.Controls.Framework;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;

    internal class RemoteApplicationImplementation : IApplication, IDisposable
    {
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
}
