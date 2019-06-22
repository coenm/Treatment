namespace CoenM.ZeroMq.ProxyExt
{
    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using ZeroMQ;

    public class ZKeySocket
    {
        public ZKeySocket([NotNull] ZSocket socket, [NotNull] string key, [NotNull] string address)
        {
            Guard.NotNull(socket, nameof(socket));
            Guard.NotNullOrWhiteSpace(key, nameof(key));
            Guard.NotNullOrWhiteSpace(address, nameof(address));

            Socket = socket;
            Key = key;
            Address = address;
        }

        public string Key { get; }

        public string Address { get; }

        public ZSocket Socket { get; }

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
    }
}
