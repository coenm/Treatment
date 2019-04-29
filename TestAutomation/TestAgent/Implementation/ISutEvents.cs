namespace TestAgent.Implementation
{
    using System;

    public interface ISutEvents
    {
        IObservable<Treatment.TestAutomation.Contract.Interfaces.Events.IEvent> Events { get; }

        Treatment.TestAutomation.Contract.Interfaces.Events.IEvent Last { get; }
    }
}
