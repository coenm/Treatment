namespace Treatment.TestAutomation.Contract.Interfaces.Events
{
    using System;

    public abstract class TestElementEventBase : IEvent
    {
        private Guid Guid { get; set; }
    }
}
