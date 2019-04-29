namespace Treatment.TestAutomation.Contract.Interfaces.EventSerializers.Element
{
    using System;
    using System.Windows;

    using global::Treatment.Helpers.Guards;
    using global::Treatment.TestAutomation.Contract.Interfaces.Events;
    using global::Treatment.TestAutomation.Contract.Interfaces.Events.Element;

    using ZeroMQ;

    public class PositionUpdatedSerializer : EventSerializerBase<PositionUpdated>
    {
        public override IEvent Deserialize(ZFrame[] evt)
        {
            Guard.NotNull(evt, nameof(evt));

            if (evt.Length != 3)
                throw new ArgumentException("Unexpected number of frames.", nameof(evt));

            return new PositionUpdated
            {
                Guid = new Guid(evt[0].Read(16)),
                Point = new Point(double.Parse(evt[1].ReadString()), double.Parse(evt[2].ReadString())),
            };
        }

        public override ZFrame[] Serialize(IEvent evt)
        {
            var e = GuardSerialize(evt);

            return new[]
            {
                new ZFrame(e.Guid.ToByteArray()),
                new ZFrame(e.Point.X.ToString()),
                new ZFrame(e.Point.Y.ToString()),
            };
        }
    }
}
