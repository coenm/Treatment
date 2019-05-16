namespace Treatment.TestAutomation.TestRunner.Framework.RemoteImplementations
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    using Contract.Interfaces.Treatment;
    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Application;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;

    internal class RemoteTreatmentApplication : ITreatmentApplication, IDisposable
    {
        private readonly CompositeDisposable disposable;

        public RemoteTreatmentApplication([NotNull] IApplicationEvents applicationEvents)
        {
            Guard.NotNull(applicationEvents, nameof(applicationEvents));

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
            };
        }

        public event EventHandler<ApplicationActivated> Activated;

        public event EventHandler<ApplicationDeactivated> Deactivated;

        public event EventHandler<ApplicationExit> Exit;

        public event EventHandler<ApplicationStarted> Startup;

        public bool Created { get; private set; }

        public ApplicationActivationState State { get; private set; }

        public IMainWindow MainWindow { get; } //todo coenm

        public void Dispose()
        {
            disposable?.Dispose();
        }
    }
}
