namespace Treatment.Plugin.TestAutomation.UI.Infrastructure
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Settings;
    using Treatment.TestAutomation.Contract.Interfaces.Events;
    using Treatment.TestAutomation.Contract.Serializer;
    using TreatmentZeroMq.ContextService;
    using TreatmentZeroMq.Helpers;
    using ZeroMQ;

    internal class ZeroMqEventPublisher : IEventPublisher, IDisposable
    {
        [NotNull] private readonly object syncLock = new object();
        [NotNull] private readonly IZeroMqContextService contextService;
        [NotNull] private readonly ITestAutomationSettings settings;
        [CanBeNull] private ZSocket socket;

        public ZeroMqEventPublisher(
            [NotNull] IZeroMqContextService contextService,
            [NotNull] ITestAutomationSettings settings)
        {
            Guard.NotNull(contextService, nameof(contextService));
            Guard.NotNull(settings, nameof(settings));

            this.contextService = contextService;
            this.settings = settings;
        }

        public Task PublishAsync(IEvent evt)
        {
            if (evt == null)
                return Task.CompletedTask;

            Initialize();

            var type = string.Empty;
            var payload = string.Empty;
            var frames = new ZFrame[0];

            try
            {
                (type, payload) = EventSerializer.Serialize(evt);

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

            if (!socket.Send(new ZMessage(frames), ZSocketFlags.DontWait, out _))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public void Dispose()
        {
            socket?.Dispose();
            socket = null;
        }

        private void Initialize()
        {
            if (socket != null)
                return;

            lock (syncLock)
            {
                if (socket != null)
                    return;

                var ctx = contextService.GetContext();
                socket = new ZSocket(ctx, ZSocketType.PUB)
                {
                    Linger = TimeSpan.Zero,
                };

                socket.Bind(settings.ZeroMqEventPublishSocket);

                ZmqConnection.GiveZeroMqTimeToFinishConnectOrBind();
            }
        }
    }
}
