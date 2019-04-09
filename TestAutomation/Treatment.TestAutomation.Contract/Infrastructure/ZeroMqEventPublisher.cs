namespace Treatment.TestAutomation.Contract.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Helpers.Guards;
    using JetBrains.Annotations;
    using ZeroMQ;

    public class ZeroMqEventPublisher : IEventPublisher, IDisposable
    {
        [NotNull] private readonly IZeroMqContextService contextService;
        [CanBeNull] private ZSocket socket;
        private readonly object syncLock = new object();

        public ZeroMqEventPublisher([NotNull] IZeroMqContextService contextService)
        {
            Guard.NotNull(contextService, nameof(contextService));

            this.contextService = contextService;
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

                socket.Bind("tcp://*:7770");

                Thread.Sleep(1);
            }
        }

        public Task PublishAsync(TestAutomationEvent evt)
        {
            Initialize();

            var frames = new List<ZFrame>
            {
                new ZFrame(evt.Control),
                new ZFrame(evt.EventName),
                new ZFrame(evt.Payload.ToString()),
            };

            ZError error;

            if (!socket.Send(new ZMessage(frames), ZSocketFlags.DontWait, out error))
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
