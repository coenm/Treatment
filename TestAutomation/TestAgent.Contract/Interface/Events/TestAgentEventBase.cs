namespace TestAgent.Contract.Interface.Events
{
    using System;

    public abstract class TestAgentEventBase : ITestAgentEvent
    {
        public Guid Guid { get; set; }
    }
}