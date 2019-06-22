namespace CoenM.ZeroMq.Socket
{
    using System;

    using CoenM.ZeroMq.ContextService;
    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using ZeroMQ;

    public class DefaultSocketFactory : IZeroMqSocketFactory
    {
        [NotNull] private readonly IZeroMqContextService contextService;

        public DefaultSocketFactory([NotNull] IZeroMqContextService contextService)
        {
            Guard.NotNull(contextService, nameof(contextService));
            this.contextService = contextService;
        }

        public ZSocket Create(ZSocketType socketType)
        {
            return new ZSocket(contextService.GetContext(), socketType)
            {
                Linger = TimeSpan.Zero,
                TcpKeepAlive = TcpKeepaliveBehaviour.Enable,
            };
        }
    }
}
