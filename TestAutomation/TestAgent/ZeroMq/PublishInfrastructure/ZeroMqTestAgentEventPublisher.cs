namespace TestAgent.ZeroMq.PublishInfrastructure
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using CoenM.ZeroMq.ContextService;
    using CoenM.ZeroMq.Helpers;
    using JetBrains.Annotations;
    using TestAgent.Contract.Interface.Events;
    using TestAgent.Contract.Serializer;
    using Treatment.Helpers.Guards;
    using ZeroMQ;

    internal class ZeroMqTestAgentEventPublisher : ITestAgentEventPublisher, IDisposable
    {
        [NotNull] private readonly object syncLock = new object();
        [NotNull] private readonly IZeroMqContextService contextService;
        [NotNull] private readonly string endpoint;
        [CanBeNull] private ZSocket socket;

        public ZeroMqTestAgentEventPublisher(
            [NotNull] IZeroMqContextService contextService,
            [NotNull] string endpoint)
        {
            Guard.NotNull(contextService, nameof(contextService));
            Guard.NotNullOrWhiteSpace(endpoint, nameof(endpoint));

            this.contextService = contextService;
            this.endpoint = endpoint;
        }

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse", Justification = "Input validation despite of [NotNull] attribute.")]
        [SuppressMessage("ReSharper", "HeuristicUnreachableCode", Justification = "Input validation despite of [NotNull] attribute.")]
        public Task PublishAsync(Guid guid, ITestAgentEvent evt)
        {
            if (evt == null)
                return Task.CompletedTask;

            evt.Guid = guid;

            return PublishAsync(evt);
        }

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse", Justification = "Input validation despite of [NotNull] attribute.")]
        [SuppressMessage("ReSharper", "HeuristicUnreachableCode", Justification = "Input validation despite of [NotNull] attribute.")]
        public Task PublishAsync(ITestAgentEvent evt)
        {
            if (evt == null)
                return Task.CompletedTask;

            ZFrame[] frames;

            try
            {
                var (type, payload) = TestAgentEventSerializer.Serialize(evt);

                frames = new[]
                {
                    new ZFrame(type),
                    new ZFrame(payload),
                };
            }
            catch (Exception e)
            {
                frames = new[]
                {
                    new ZFrame(e.GetType().FullName),
                    new ZFrame(e.Message),
                };
            }

            if (!GetSocket().Send(new ZMessage(frames), ZSocketFlags.DontWait, out _))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public void Dispose()
        {
            lock (syncLock)
            {
                socket?.Dispose();
                socket = null;
            }
        }

        [NotNull]
        private ZSocket GetSocket()
        {
            if (socket != null)
                return socket;

            lock (syncLock)
            {
                if (socket != null)
                    return socket;

                var ctx = contextService.GetContext();
                socket = new ZSocket(ctx, ZSocketType.PUB)
                {
                    Linger = TimeSpan.Zero,
                    TcpKeepAlive = TcpKeepaliveBehaviour.Enable,
                    SendHighWatermark = 10_000,
                };

                socket.Connect(endpoint);

                ZmqConnection.GiveZeroMqTimeToFinishConnectOrBind();

                return socket;
            }
        }
    }
}
