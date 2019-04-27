namespace Treatment.Plugin.TestAutomation.UI.Infrastructure
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Treatment.TestAutomation.Contract.Interfaces.Events;

    public interface IEventPublisher
    {
        Task PublishAsync([NotNull] IEvent evt);
    }

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
    }
}
