namespace TestAgent.Implementation
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using Interface;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Treatment.Helpers.Guards;
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
        private static ZMessage Serialize([NotNull] IResponse rsp)
        {
            return new ZMessage
            {
                new ZFrame(rsp.GetType().FullName),
                new ZFrame(JsonConvert.SerializeObject(rsp))
            };
        }

        [NotNull]
        private static IRequest Deserialize([NotNull] ZMessage message)
        {
            Type payloadType;
            var typeString = message[0].ReadString();
            var payload = message[1].ReadString();

            try
            {
                payloadType = TypeCache.GetOrAdd(
                    typeString,
                    _ => AppDomain.CurrentDomain
                        .GetAssemblies()
                        .Select(a => a.GetType(typeString, false))
                        .Single(t => t != null));
            }
            catch (Exception)
            {
                throw;
                // var errorMsg = $"Type not found for message {req.PayloadType}.";
                // _logger.Warn(errorMsg + " Return Exception message");
                //
                // var ex = new Exception(errorMsg);
                // return new ZmqExceptionResponseMessage(req, ex);
            }

            if (string.IsNullOrWhiteSpace(payload))
            {
                throw new Exception("payload is null");
            }

            return JsonConvert.DeserializeObject(payload, payloadType) as IRequest ?? throw new InvalidOperationException();
        }
    }
}
