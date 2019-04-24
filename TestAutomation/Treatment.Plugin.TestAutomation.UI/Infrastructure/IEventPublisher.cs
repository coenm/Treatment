namespace Treatment.Plugin.TestAutomation.UI.Infrastructure
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    public interface IEventPublisher
    {
        Task PublishAsync([NotNull] TestAutomationEvent evt);
    }

    public static class EventPublisherExtension
    {
        public static Task PublishNewContol(this IEventPublisher publisher, Guid guid, Type type, Guid parent)
        {
            return publisher.PublishAsync(new TestAutomationEvent()
            {
                EventName = "CREATE",
                Control = null,
                Payload = $"new: {guid}; type: {type.Name}; parent: {parent};"
            });
        }
    }
}
