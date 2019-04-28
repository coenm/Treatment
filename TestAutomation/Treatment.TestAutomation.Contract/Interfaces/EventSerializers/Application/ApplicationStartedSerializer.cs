namespace Treatment.TestAutomation.Contract.Interfaces.EventSerializers.Application
{
    using Events;
    using Events.Application;
    using Helpers.Guards;
    using ZeroMQ;

    public class ApplicationStartedSerializer : EventSerializerBase<ApplicationStarted>
    {
        public override IEvent Deserialize(ZFrame[] evt)
        {
            Guard.NotNull(evt, nameof(evt));
            return new ApplicationStarted();
        }

        public override ZFrame[] Serialize(IEvent evt)
        {
            Guard.NotNull(evt, nameof(evt));
            return new ZFrame[0];
        }
    }
}
