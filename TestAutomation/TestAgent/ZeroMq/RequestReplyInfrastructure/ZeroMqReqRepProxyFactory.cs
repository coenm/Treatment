namespace TestAgent.ZeroMq.RequestReplyInfrastructure
{
    using Treatment.TestAutomation.Contract.ZeroMq;

    public class ZeroMqReqRepProxyFactory : IZeroMqReqRepProxyFactory
    {
        private readonly IZeroMqContextService _contextService;
        private readonly ZeroMqReqRepProxyConfig _config;
        private readonly ILogger _logger;

        public ZeroMqReqRepProxyFactory(IZeroMqContextService contextService, ZeroMqReqRepProxyConfig config, ILogger logger)
        {
            _contextService = contextService;
            _config = config;
            _logger = logger;
        }

        public ZeroMqReqRepProxyConfig GetConfig()
        {
            return _config;
        }

        public ZeroMqReqRepProxyService Create()
        {
            return new ZeroMqReqRepProxyService(_contextService.GetContext(), _config, _logger);
        }
    }
}
