namespace Treatment.TestAutomation.Contract.Interfaces.EventSerializers
{
    using System;
    using Events;
    using Helpers.Guards;
    using ZeroMQ;


    public abstract class EventSerializerBase<T> : IEventSerializer
    {
        public int Priority { get; } = 10;

        public abstract IEvent Deserialize(ZFrame[] evt);

        public abstract ZFrame[] Serialize(IEvent evt);

        public bool CanSerialize(IEvent evt) => evt != null && evt.GetType() == typeof(T);

        protected T GuardSerialize(IEvent evt)
        {
            Guard.NotNull(evt, nameof(evt));

            if (!(evt is T e))
                throw new ArgumentException("Wrong type", nameof(evt));

            return e;
        }
    }
}
