namespace Treatment.Plugin.TestAutomation.UI.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Settings;
    using Treatment.TestAutomation.Contract.Interfaces.Events;
    using Treatment.TestAutomation.Contract.Interfaces.EventSerializers;
    using Treatment.TestAutomation.Contract.ZeroMq;
    using ZeroMQ;

    internal class ZeroMqEventPublisher : IEventPublisher, IDisposable
    {
        [NotNull] private readonly object syncLock = new object();
        [NotNull] private readonly IZeroMqContextService contextService;
        [NotNull] private readonly ITestAutomationSettings settings;
        [NotNull] private readonly IEnumerable<IEventSerializer> serializers;
        [CanBeNull] private ZSocket socket;

        public ZeroMqEventPublisher(
            [NotNull] IZeroMqContextService contextService,
            [NotNull] ITestAutomationSettings settings,
            [NotNull] IEnumerable<IEventSerializer> serializers)
        {
            Guard.NotNull(contextService, nameof(contextService));
            Guard.NotNull(settings, nameof(settings));
            Guard.NotNull(serializers, nameof(serializers));

            this.contextService = contextService;
            this.settings = settings;
            this.serializers = serializers;
        }

        public Task PublishAsync(IEvent evt)
        {
            Initialize();

            var s = serializers.OrderBy(x => x.Priority).FirstOrDefault(x => x.CanSerialize(evt));
            if (s != null)
            {
                var frames = new ZFrame[]
                {
                    new ZFrame(s.GetType().FullName),
                }.Concat(s.Serialize(evt));

                if (!socket.Send(new ZMessage(frames), ZSocketFlags.DontWait, out _))
                {
                    return Task.FromResult(false);
                }

                return Task.FromResult(true);
            }

            if (evt is TestAutomationEvent taEvt)
            {
                var frames = new List<ZFrame>
                {
                    new ZFrame(taEvt.Control ?? string.Empty),
                    new ZFrame(taEvt.EventName ?? string.Empty),
                    new ZFrame(taEvt.Payload?.ToString() ?? string.Empty),
                    new ZFrame(DateTime.Now.ToString("HH:mm:ss:fff")),
                };

                if (!socket.Send(new ZMessage(frames), ZSocketFlags.DontWait, out _))
                {
                    return Task.FromResult(false);
                }
            }
            else
            {
                var frames = new List<ZFrame>
                {
                    new ZFrame(evt.GetType().FullName),
                    new ZFrame(DateTime.Now.ToString("HH:mm:ss:fff")),
                };

                if (!socket.Send(new ZMessage(frames), ZSocketFlags.DontWait, out _))
                {
                    return Task.FromResult(false);
                }
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

                Thread.Sleep(1);
            }
        }
    }
}
