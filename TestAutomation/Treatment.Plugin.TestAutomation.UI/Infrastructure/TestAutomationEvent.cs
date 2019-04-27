namespace Treatment.Plugin.TestAutomation.UI.Infrastructure
{
    using Treatment.TestAutomation.Contract.Interfaces.Events;

    public class TestAutomationEvent : IEvent
    {
        public string Control { get; set; }

        public string EventName { get; set; }

        public object Payload { get; set; }
    }
}
