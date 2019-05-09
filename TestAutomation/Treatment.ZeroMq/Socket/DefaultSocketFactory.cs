namespace TreatmentZeroMq.Socket
{
    using System;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using TreatmentZeroMq.ContextService;
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
            var result = new ZSocket(contextService.GetContext(), socketType)
            {
                Linger = TimeSpan.Zero,
                TcpKeepAlive = TcpKeepaliveBehaviour.Enable,
            };

            return result;
        }
    }
}
