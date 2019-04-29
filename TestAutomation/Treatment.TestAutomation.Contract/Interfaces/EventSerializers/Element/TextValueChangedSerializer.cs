namespace Treatment.TestAutomation.Contract.Interfaces.EventSerializers.Element
{
    using System;

    using global::Treatment.Helpers.Guards;
    using global::Treatment.TestAutomation.Contract.Interfaces.Events;
    using global::Treatment.TestAutomation.Contract.Interfaces.Events.Element;

    using ZeroMQ;

    public class TextValueChangedSerializer : EventSerializerBase<TextValueChanged>
    {
        public override IEvent Deserialize(ZFrame[] evt)
        {
            Guard.NotNull(evt, nameof(evt));

            if (evt.Length != 2)
                throw new ArgumentException("Unexpected number of frames.", nameof(evt));

            return new TextValueChanged
                   {
                       Guid = new Guid(evt[0].Read(16)),
                       Text = evt[1].ReadString(),
                   };
        }

        public override ZFrame[] Serialize(IEvent evt)
        {
            var e = GuardSerialize(evt);

            return new[]
                   {
                       new ZFrame(e.Guid.ToByteArray()),
                       new ZFrame(e.Text),
                   };
        }
    }
}
