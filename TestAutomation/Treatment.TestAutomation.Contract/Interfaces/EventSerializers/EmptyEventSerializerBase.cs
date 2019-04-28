namespace Treatment.TestAutomation.Contract.Interfaces.EventSerializers
{
    using Events;
    using Helpers.Guards;
    using ZeroMQ;

    public abstract class EmptyEventSerializerBase<T> : EventSerializerBase<T> where T : IEvent, new()
    {
        public override IEvent Deserialize(ZFrame[] evt)
        {
            Guard.NotNull(evt, nameof(evt));
            return new T();
        }

        public override ZFrame[] Serialize(IEvent evt)
        {
            Guard.NotNull(evt, nameof(evt));
            return new ZFrame[0];
        }
    }
}
