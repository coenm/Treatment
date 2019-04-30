namespace TestAgent
{
    using Implementation;
    using JetBrains.Annotations;
    using SimpleInjector;
    using TestAgent.ZeroMq;
    using TestAgent.ZeroMq.PublishInfrastructure;
    using TestAgent.ZeroMq.RequestReplyInfrastructure;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.EventSerializers;
    using Treatment.TestAutomation.Contract.ZeroMq;

    internal static class Bootstrapper
    {
        public static void Bootstrap([NotNull] Container container, [NotNull] string endpointRequestResponse, [NotNull] string endpointPublish)
        {
            Guard.NotNull(container, nameof(container));
            Guard.NotNull(endpointRequestResponse, nameof(endpointRequestResponse));
            Guard.NotNull(endpointPublish, nameof(endpointPublish));

            // sut context
            container.RegisterSingleton<ISutContext, SutContext>();

            // Events from the TestAutomation plugin (specified in TestAutomation contact).
            container.Collection.Register(typeof(IEventSerializer), typeof(IEventSerializer).Assembly);

            // all possible request handlers
            container.Collection.Register(typeof(IRequestHandler), typeof(IRequestHandler).Assembly);

            container.Register<IRequestDispatcher, RequestDispatcher>(Lifestyle.Transient);


            container.Register<ILogger, EmptyLogger>(Lifestyle.Singleton);

            BootstrapZeroMq(container, endpointRequestResponse, endpointPublish);
        }

        private static void BootstrapZeroMq([NotNull] Container container, [NotNull] string endpointReqRsp, [NotNull] string endpointPubSub)
        {
            // Ensures ZeroMq Context.
            container.RegisterSingleton<IZeroMqContextService, ZeroMqContextService>();
            container.Register<IZeroMqRequestDispatcher, ZeroMqZeroMqRequestDispatcher>(Lifestyle.Transient);

            container.Register<ZeroMqPublishProxyConfig>(() => new ZeroMqPublishProxyConfig(endpointPubSub, "inproc://pub-sub"));
            container.Register<IZeroMqPublishProxyFactory, ZeroMqPublishProxyFactory>(Lifestyle.Transient);

            container.Register<ZeroMqReqRepProxyConfig>(() => new ZeroMqReqRepProxyConfig(endpointReqRsp, "inproc://publish"));
            container.Register<IZeroMqReqRepProxyFactory, ZeroMqReqRepProxyFactory>(Lifestyle.Transient);
        }
    }
}
