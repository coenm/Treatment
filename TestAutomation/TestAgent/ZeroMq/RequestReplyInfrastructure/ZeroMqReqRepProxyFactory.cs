namespace TestAgent.ZeroMq.RequestReplyInfrastructure
{
    using Treatment.TestAutomation.Contract.ZeroMq;

    public class ZeroMqReqRepProxyFactory : IZeroMqReqRepProxyFactory
    {
        private readonly IZeroMqContextService contextService;
        private readonly ZeroMqReqRepProxyConfig config;
        private readonly ILogger logger;

        public ZeroMqReqRepProxyFactory(IZeroMqContextService contextService, ZeroMqReqRepProxyConfig config, ILogger logger)
        {
            this.contextService = contextService;
            this.config = config;
            this.logger = logger;
        }

        public ZeroMqReqRepProxyConfig GetConfig()
        {
            return config;
        }

        public ZeroMqReqRepProxyService Create()
        {
            return new ZeroMqReqRepProxyService(contextService.GetContext(), config, logger);
        }
    }
}
