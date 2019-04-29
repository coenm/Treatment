namespace TestAgent.Implementation
{
    using System.Reactive.Subjects;
    using JetBrains.Annotations;
    using Treatment.TestAutomation.Contract.Interfaces.Events;

    public interface ISutEventsPublisher
    {
        [NotNull]
        Subject<IEvent> Subject { get; }
    }
}
