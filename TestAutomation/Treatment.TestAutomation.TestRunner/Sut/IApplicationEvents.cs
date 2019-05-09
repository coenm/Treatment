namespace Treatment.TestAutomation.TestRunner.Sut
{
    using System;

    using JetBrains.Annotations;
    using Treatment.TestAutomation.Contract.Interfaces.Events;

    public interface IApplicationEvents
    {
        [NotNull]
        IObservable<IEvent> Events { get; }
    }
}
