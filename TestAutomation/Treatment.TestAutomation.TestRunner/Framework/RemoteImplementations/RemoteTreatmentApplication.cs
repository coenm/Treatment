namespace Treatment.TestAutomation.TestRunner.Framework.RemoteImplementations
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

            disposable = new CompositeDisposable
            {
                applicationEvents.Events
                    .Where(x => x is ApplicationStarted)
                    .Subscribe(ev => { Created = true; }),
            };
        }

        public bool Created { get; set; }

        public void Dispose()
        {
            disposable?.Dispose();
        }
    }
}
