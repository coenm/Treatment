namespace TestAgent.ZeroMq.PublishInfrastructure
{
    using CoenM.ZeroMq.ContextService;
    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;

    public class ZeroMqPublishProxyFactory : IZeroMqPublishProxyFactory
    {
        [NotNull] private readonly IZeroMqContextService contextService;
        [NotNull] private readonly ZeroMqPublishProxyConfig config;

        public ZeroMqPublishProxyFactory(
            [NotNull] IZeroMqContextService contextService,
            [NotNull] ZeroMqPublishProxyConfig config)
        {
            Guard.NotNull(contextService, nameof(contextService));
            Guard.NotNull(config, nameof(config));

            this.contextService = contextService;
            this.config = config;
        }

        public ZeroMqPublishProxyConfig GetConfig()
        {
            return config;
        }

        public ZeroMqPublishProxyService Create()
        {
            return new ZeroMqPublishProxyService(contextService.GetContext(), config);
        }
    }
}
