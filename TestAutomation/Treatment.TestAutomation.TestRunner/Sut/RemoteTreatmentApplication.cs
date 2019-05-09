namespace Treatment.TestAutomation.TestRunner.Sut
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    using Treatment.TestAutomation.Contract.Interfaces.Events.Application;

    internal class RemoteTreatmentApplication : ITreatmentApplication, IDisposable
    {
        private readonly IApplicationEvents applicationEvents;
        private readonly CompositeDisposable disposable;

        public RemoteTreatmentApplication(IApplicationEvents applicationEvents)
        {
            disposable = new CompositeDisposable();
            this.applicationEvents = applicationEvents;

            disposable.Add(applicationEvents.Events
                             .Where(x => x is ApplicationStarted)
                             .Subscribe(ev =>
                                        {
                                            Created = true;
                                        }));
        }

        public bool Created { get; set; }

        public void Dispose()
        {
            disposable?.Dispose();
        }
    }
}
