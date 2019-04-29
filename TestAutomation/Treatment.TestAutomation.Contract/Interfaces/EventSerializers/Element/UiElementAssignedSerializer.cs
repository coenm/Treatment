namespace Treatment.TestAutomation.Contract.Interfaces.EventSerializers.Element
{
    using System;

    using global::Treatment.Helpers.Guards;
    using global::Treatment.TestAutomation.Contract.Interfaces.Events;
    using global::Treatment.TestAutomation.Contract.Interfaces.Events.Element;

    using ZeroMQ;

    public class UiElementAssignedSerializer : EventSerializerBase<UiElementAssigned>
    {
        public override IEvent Deserialize(ZFrame[] evt)
        {
            Guard.NotNull(evt, nameof(evt));

            if (evt.Length != 3)
                throw new ArgumentException("Unexpected number of frames.", nameof(evt));

            return new UiElementAssigned
            {
                Guid = new Guid(evt[0].Read(16)),
                PropertyName = evt[1].ReadString(),
                ChildElement = new Guid(evt[2].Read(16)),
            };
        }

        public override ZFrame[] Serialize(IEvent evt)
        {
            var e = GuardSerialize(evt);

            return new[]
                   {
                       new ZFrame(e.Guid.ToByteArray()),
                       new ZFrame(e.PropertyName),
                       new ZFrame(e.ChildElement.ToByteArray()),
                   };
        }
    }
}
