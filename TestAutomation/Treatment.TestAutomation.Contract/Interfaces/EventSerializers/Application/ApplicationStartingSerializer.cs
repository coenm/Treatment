namespace Treatment.TestAutomation.Contract.Interfaces.EventSerializers.Application
{
    using System;
    using Events;
    using Events.Application;
    using Helpers.Guards;
    using ZeroMQ;

    public class ApplicationStartingSerializer : EventSerializerBase<ApplicationStarting>
    {
        public override IEvent Deserialize(ZFrame[] evt)
        {
            Guard.NotNull(evt, nameof(evt));

            if (evt.Length != 1)
                throw new ArgumentException("Unexpected number of frames.", nameof(evt));

            return new ApplicationStarting
            {
                CountDown = evt[0].ReadInt32()
            };
        }

        public override ZFrame[] Serialize(IEvent evt)
        {
            var e = GuardSerialize(evt);

            return new[]
            {
                new ZFrame(e.CountDown)
            };
        }
    }
}
