namespace TestAgent.ZeroMq.PublishInfrastructure
{
    using Treatment.TestAutomation.Contract.ZeroMq;

    public class ZeroMqPublishProxyFactory : IZeroMqPublishProxyFactory
    {
        private readonly IZeroMqContextService contextService;
        private readonly ZeroMqPublishProxyConfig config;
        private readonly ILogger logger;

    public ZeroMqPublishProxyFactory(IZeroMqContextService contextService, ZeroMqPublishProxyConfig config, ILogger logger)
        {
            this.contextService = contextService;
            this.config = config;
            this.logger = logger;
        }

        public ZeroMqPublishProxyConfig GetConfig()
        {
            return config;
        }

        public ZeroMqPublishProxyService Create()
        {
            return new ZeroMqPublishProxyService(contextService.GetContext(), config, logger);
        }
    }
}
