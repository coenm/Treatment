namespace Treatment.TestAutomation.Contract.Interfaces.EventSerializers.Application
{
    using System;
    using Events;
    using Events.Application;
    using Helpers.Guards;
    using ZeroMQ;

    public class ApplicationStoppedSerializer : EventSerializerBase<ApplicationStopped>
    {
        public override IEvent Deserialize(ZFrame[] evt)
        {
            Guard.NotNull(evt, nameof(evt));

            if (evt.Length != 1)
                throw new ArgumentException("Unexpected number of frames.", nameof(evt));

            return new ApplicationStopped();
        }

        public override ZFrame[] Serialize(IEvent evt)
        {
            var _ = GuardSerialize(evt);

            return new ZFrame[0];
        }
    }
}
