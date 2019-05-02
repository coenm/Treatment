namespace TestAgent
{
    using Implementation;
    using JetBrains.Annotations;
    using SimpleInjector;
    using TestAgent.ZeroMq.PublishInfrastructure;
    using TestAgent.ZeroMq.RequestReplyInfrastructure;
    using Treatment.Helpers.Guards;
    using TreatmentZeroMq.ContextService;

    internal static class Bootstrapper
    {
        public static void Bootstrap([NotNull] Container container,
            [NotNull] string endpointRequestResponse,
            [NotNull] string endpointPublish,
            [NotNull] string sutPublishPort)
        {
            Guard.NotNull(container, nameof(container));
            Guard.NotNull(endpointRequestResponse, nameof(endpointRequestResponse));
            Guard.NotNull(endpointPublish, nameof(endpointPublish));
            Guard.NotNull(sutPublishPort, nameof(sutPublishPort));

            // sut context
            container.RegisterSingleton<ISutContext, SutContext>();

            // all possible request handlers
            container.Collection.Register(typeof(IRequestHandler), typeof(IRequestHandler).Assembly);

            container.Register<IRequestDispatcher, RequestDispatcher>(Lifestyle.Transient);

            BootstrapZeroMq(container,
                endpointRequestResponse,
                endpointPublish,
                sutPublishPort);
        }

        private static void BootstrapZeroMq(
            [NotNull] Container container,
            [NotNull] string endpointReqRsp,
            [NotNull] string endpointPubSub,
            [NotNull] string sutPublishPort)
        {
            // Ensures ZeroMq Context.
            container.RegisterSingleton<IZeroMqContextService, ZeroMqContextService>();
            container.Register<IZeroMqRequestDispatcher, ZeroMqZeroMqRequestDispatcher>(Lifestyle.Transient);

            container.Register<ZeroMqPublishProxyConfig>(() => new ZeroMqPublishProxyConfig(
                new []{ endpointPubSub },
                new[] { $"tcp://*:{sutPublishPort}", "inproc://publish" }));

            container.Register<IZeroMqPublishProxyFactory, ZeroMqPublishProxyFactory>(Lifestyle.Transient);

            container.Register<ZeroMqReqRepProxyConfig>(() => new ZeroMqReqRepProxyConfig(
                new[] { endpointReqRsp },
                new[] { "inproc://reqrsp" }),
                Lifestyle.Singleton);

            container.Register<IZeroMqReqRepProxyFactory, ZeroMqReqRepProxyFactory>(Lifestyle.Singleton);
        }
    }
}
