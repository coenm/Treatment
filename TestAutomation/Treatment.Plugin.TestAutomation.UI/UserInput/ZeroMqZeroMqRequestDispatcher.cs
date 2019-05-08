namespace Treatment.Plugin.TestAutomation.UI.UserInput
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using global::TestAutomation.Input.Contract.Interface;
    using global::TestAutomation.Input.Contract.Serializer;
    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using TreatmentZeroMq.Worker;
    using ZeroMQ;

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
        private static ZMessage Serialize([NotNull] IInputResponse rsp)
        {
            var (type, payload) = InputRequestResponseSerializer.Serialize(rsp);

            return new ZMessage
            {
                new ZFrame(type),
                new ZFrame(payload),
            };
        }

        [CanBeNull]
        private static IInputRequest Deserialize([NotNull] ZMessage message)
        {
            return InputRequestResponseSerializer.DeserializeRequest(
                message[0].ReadString(),
                message[1].ReadString());
        }
    }
}
