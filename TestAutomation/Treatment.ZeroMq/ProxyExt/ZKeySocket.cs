namespace TreatmentZeroMq.ProxyExt
{
    using System;
    using ZeroMQ;

    public class ZKeySocket
    {
        public ZKeySocket(ZSocket socket, string key, string address)
        {
            Socket = socket ?? throw new ArgumentNullException(nameof(socket));
            Key = key ?? throw new ArgumentNullException(nameof(Key));
            Address = address ?? throw new ArgumentNullException(nameof(Address));
        }

        public string Key { get; }

        public string Address { get; }

        public bool ShouldUseSocket(ZFrame msg)
        {
            if (msg == null)
                return false;

            using (var dup = msg.Duplicate())
            {
                var providedKey = dup.ReadString();
                return Key == providedKey;
            }
        }

        public ZSocket Socket { get; }
    }
}
