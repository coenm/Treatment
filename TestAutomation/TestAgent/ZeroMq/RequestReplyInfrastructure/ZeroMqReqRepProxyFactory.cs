namespace TestAgent.ZeroMq.RequestReplyInfrastructure
{
    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using TreatmentZeroMq.ContextService;

    public class ZeroMqReqRepProxyFactory : IZeroMqReqRepProxyFactory
    {
        [NotNull] private readonly IZeroMqContextService contextService;
        [NotNull] private readonly ZeroMqReqRepProxyConfig config;

        public ZeroMqReqRepProxyFactory(
            [NotNull] IZeroMqContextService contextService,
            [NotNull] ZeroMqReqRepProxyConfig config)
        {
            Guard.NotNull(contextService, nameof(contextService));
            Guard.NotNull(config, nameof(contextService));

            this.contextService = contextService;
            this.config = config;
        }

        public ZeroMqReqRepProxyConfig GetConfig()
        {
            return config;
        }

        public ZeroMqReqRepProxyService Create()
        {
            return new ZeroMqReqRepProxyService(contextService.GetContext(), config);
        }
    }
}
