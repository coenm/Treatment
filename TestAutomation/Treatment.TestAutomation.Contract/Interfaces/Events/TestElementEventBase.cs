namespace Treatment.TestAutomation.Contract.Interfaces.Events
{
    using System;

    public abstract class TestElementEventBase : IEvent
    {
        public Guid Guid { get; set; }
    }
}
