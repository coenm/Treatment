namespace CoenM.ZeroMq.Helpers
{
    using System.Collections.Generic;

    using Treatment.Helpers.Guards;
    using ZeroMQ;

    public static class ZmqPolls
    {
        public static List<ZPollItem> CreateReceiverPolls(int count)
        {
            Guard.MustBeGreaterThan(count, 0, nameof(count));

            var result = new List<ZPollItem>();
            for (uint i = 0; i < count; i++)
                result.Add(ZPollItem.CreateReceiver());
            return result;
        }
    }
}
