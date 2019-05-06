namespace Treatment.Plugin.TestAutomation.UI.UserInput
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using TreatmentZeroMq.Worker;
    using ZeroMQ;

    using global::TestAutomation.Contract.Input.Interface;
    using global::TestAutomation.Contract.Input.Serializer;

    [UsedImplicitly]
    public class ZeroMqZeroMqRequestDispatcher : IZeroMqRequestDispatcher
    {
        private static readonly ConcurrentDictionary<string, Type> TypeCache = new ConcurrentDictionary<string, Type>();
        [NotNull] private readonly IRequestDispatcher requestDispatcher;

        public ZeroMqZeroMqRequestDispatcher([NotNull] IRequestDispatcher requestDispatcher)
        {
            Guard.NotNull(requestDispatcher, nameof(requestDispatcher));
            this.requestDispatcher = requestDispatcher;
        }

        public async Task<ZMessage> ProcessAsync([NotNull] ZMessage message)
        {
            var req = Deserialize(message);

            var rsp = await requestDispatcher.ProcessAsync(req);

            return Serialize(rsp);
        }

        [NotNull]
        private static ZMessage Serialize([NotNull] IResponse rsp)
        {
            var (type, payload) = RequestResponseSerializer.Serialize(rsp);

            return new ZMessage
            {
                new ZFrame(type),
                new ZFrame(payload),
            };
        }

        [CanBeNull]
        private static IRequest Deserialize([NotNull] ZMessage message)
        {
            return RequestResponseSerializer.DeserializeRequest(
                message[0].ReadString(),
                message[1].ReadString());
        }
    }
}
