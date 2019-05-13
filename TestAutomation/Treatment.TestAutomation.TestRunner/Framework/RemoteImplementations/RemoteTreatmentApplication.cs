﻿namespace Treatment.TestAutomation.TestRunner.Framework.RemoteImplementations
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

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
                        Startup?.Invoke(this, EventArgs.Empty);
                    }),

                applicationEvents.Events
                    .Where(x => x is ApplicationActivated)
                    .Subscribe(ev =>
                    {
                        State = ApplicationActivationState.Activated;
                        Activated?.Invoke(this, EventArgs.Empty);
                    }),

                applicationEvents.Events
                    .Where(x => x is ApplicationDeactivated)
                    .Subscribe(ev =>
                    {
                        State = ApplicationActivationState.Deactivated;
                        Deactivated?.Invoke(this, EventArgs.Empty);
                    }),

                applicationEvents.Events
                    .Where(x => x is ApplicationExit)
                    .Subscribe(ev => Exit?.Invoke(this, (ApplicationExit)ev)),
            };
        }

        public event EventHandler Activated;

        public event EventHandler Deactivated;

        public event EventHandler<ApplicationExit> Exit;

        public event EventHandler Startup;

        public bool Created { get; private set; }

        public ApplicationActivationState State { get; private set; }

        public void Dispose()
        {
            disposable?.Dispose();
        }
    }
}
