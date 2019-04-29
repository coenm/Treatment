namespace Treatment.Plugin.TestAutomation.UI.Infrastructure
{
    using System;
    using System.Threading.Tasks;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;

    public static class EventPublisherExtension
    {
        public static Task PublishNewControl(this IEventPublisher publisher, Guid guid, Type type, Guid parent)
        {
            return publisher.PublishAsync(new TestAutomationEvent
            {
                EventName = "CREATE",
                Control = null,
                Payload = $"new: {guid}; type: {type.Name}; parent: {parent};"
            });
        }

        public static Task PublishAssignedAsync(this IEventPublisher publisher, Guid parent, string propertyName, Guid element)
        {
            return publisher.PublishAsync(
                new UiElementAssigned
                {
                    Guid = parent,
                    PropertyName = propertyName,
                    ChildElement = element,
                });
        }
    }
}