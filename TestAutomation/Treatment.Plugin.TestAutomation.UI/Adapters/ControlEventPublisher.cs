namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using JetBrains.Annotations;

    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Events;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    public class ControlEventPublisher : IDisposable
    {
        private readonly Guid guid;
        private readonly IEventPublisher eventPublisher;
        private readonly List<EventToPublish> eventToPublishCollection;

        public ControlEventPublisher([NotNull] IControl control, Guid guid, IEventPublisher eventPublisher)
        {

            Guard.NotNull(control, nameof(control));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.guid = guid;
            this.eventPublisher = eventPublisher;

            eventToPublishCollection = new List<EventToPublish>();

            var allEvents = control.GetType().GetEvents();
            var method = typeof(ControlEventPublisher).GetMethod(nameof(Publish), BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
                return;

            foreach (var eventInfo in allEvents)
            {
                var handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, method);
                eventToPublishCollection.Add(new EventToPublish(eventInfo, control, handler));
            }
        }

        public void Dispose()
        {
            eventToPublishCollection.ForEach(c => c.Dispose());
            eventToPublishCollection.Clear();
        }

        private void Publish(object sender, IEvent e)
        {
            Console.WriteLine(e.ToString());
            eventPublisher.PublishAsync(guid, e);
        }

        private struct EventToPublish : IDisposable
        {
            private readonly EventInfo eventInfo;
            private readonly object obj;
            private readonly Delegate handler;

            public EventToPublish(EventInfo eventInfo, object obj, Delegate handler)
            {
                this.eventInfo = eventInfo;
                this.obj = obj;
                this.handler = handler;

                eventInfo.AddEventHandler(obj, handler);
            }

            public void Dispose()
            {
                eventInfo.RemoveEventHandler(obj, handler);
            }
        }
    }
}
