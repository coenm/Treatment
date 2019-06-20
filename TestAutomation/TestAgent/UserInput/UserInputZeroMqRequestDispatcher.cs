namespace TestAgent.UserInput
{
    using System.Threading.Tasks;

    using CoenM.ZeroMq.Worker;
    using JetBrains.Annotations;
    using TestAutomation.Input.Contract.Interface;
    using TestAutomation.Input.Contract.Serializer;
    using Treatment.Helpers.Guards;
    using ZeroMQ;

    [UsedImplicitly]
    public class UserInputZeroMqRequestDispatcher : IZeroMqRequestDispatcher
    {
        [NotNull] private readonly IRequestDispatcher requestDispatcher;

        public UserInputZeroMqRequestDispatcher([NotNull] IRequestDispatcher requestDispatcher)
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
