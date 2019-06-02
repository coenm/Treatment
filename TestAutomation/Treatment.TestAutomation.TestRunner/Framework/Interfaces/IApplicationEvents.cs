namespace Treatment.TestAutomation.TestRunner.Framework.Interfaces
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
