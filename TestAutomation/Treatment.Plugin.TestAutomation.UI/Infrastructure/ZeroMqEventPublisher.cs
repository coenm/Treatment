namespace Treatment.Plugin.TestAutomation.UI.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Settings;
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

        public Task PublishAsync(TestAutomationEvent evt)
        {
            Initialize();

            var frames = new List<ZFrame>
            {
                new ZFrame(evt.Control ?? string.Empty),
                new ZFrame(evt.EventName ?? string.Empty),
                new ZFrame(evt.Payload?.ToString() ?? string.Empty),
                new ZFrame(DateTime.Now.ToString("HH:mm:ss:fff")),
            };

            if (!socket.Send(new ZMessage(frames), ZSocketFlags.DontWait, out ZError _))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
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

        public void Dispose()
        {
            socket?.Dispose();
            socket = null;
        }
    }
}
