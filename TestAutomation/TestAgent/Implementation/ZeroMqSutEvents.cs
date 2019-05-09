namespace TestAgent.Implementation
{
    using System;
    using System.Reactive.Subjects;

    using JetBrains.Annotations;
    using Treatment.TestAutomation.Contract.Interfaces.Events;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Application;

    public class ZeroMqSutEvents : ISutEvents, ISutEventsPublisher
    {
        public ZeroMqSutEvents()
        {
            Subject = new Subject<IEvent>();
            Subject.Subscribe(item => Last = item);

            Subject.OnNext(new ApplicationActivated());
            Subject.OnNext(new ApplicationActivated());
            Subject.OnNext(new ApplicationActivated());
        }

        [NotNull]
        public Subject<IEvent> Subject { get; }

        [NotNull]
        public IObservable<IEvent> Events => Subject;

        public IEvent Last { get; private set; }
    }
}
