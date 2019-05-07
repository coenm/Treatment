namespace Treatment.Plugin.TestAutomation.UI.Infrastructure
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;

    public static class EventPublisherExtension
    {
        public static Task PublishNewControlCreatedAsync([NotNull] this IEventPublisher publisher, Guid guid, Type type)
        {
            Guard.NotNull(publisher, nameof(publisher));

            return publisher.PublishAsync(
                new NewControlCreated
                {
                    Guid = guid,
                    Interface = type,
                });
        }

        public static Task PublishAssignedAsync([NotNull] this IEventPublisher publisher, Guid parent, string propertyName, Guid element)
        {
            Guard.NotNull(publisher, nameof(publisher));

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
