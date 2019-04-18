namespace Treatment.Plugin.TestAutomation.UI.Infrastructure.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Settings;
    using Treatment.Helpers.Guards;
    using ZeroMQ;

    internal class ZeroMqEventPublisher : IEventPublisher, IDisposable
    {
        private readonly object syncLock = new object();
        [NotNull] private readonly IZeroMqContextService contextService;
        private readonly ITestAutomationSettings settings;
        [CanBeNull] private ZSocket socket;

        public ZeroMqEventPublisher([NotNull] IZeroMqContextService contextService, [NotNull] ITestAutomationSettings settings)
        {
            Guard.NotNull(contextService, nameof(contextService));
            Guard.NotNull(settings, nameof(settings));

            this.contextService = contextService;
            this.settings = settings;
        }

        public void Initialize()
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

                Thread.Sleep(10);
            }
        }

        public Task PublishAsync(TestAutomationEvent evt)
        {
            Initialize();

            var frames = new List<ZFrame>
            {
                new ZFrame(evt.Control ?? string.Empty),
                new ZFrame(evt.EventName ?? string.Empty),
                new ZFrame(evt.Payload?.ToString() ?? string.Empty),
            };

            if (!socket.Send(new ZMessage(frames), ZSocketFlags.DontWait, out ZError _))
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
    }
}
