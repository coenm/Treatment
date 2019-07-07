namespace TestAgent
{
    using CoenM.ZeroMq.ContextService;
    using CoenM.ZeroMq.Socket;
    using CoenM.ZeroMq.Worker;
    using JetBrains.Annotations;
    using SimpleInjector;
    using TestAgent.Implementation;
    using TestAgent.ZeroMq.PublishInfrastructure;
    using TestAgent.ZeroMq.RequestReplyInfrastructure;
    using Treatment.Helpers.Guards;

    using IRequestDispatcher = TestAgent.Implementation.IRequestDispatcher;
    using RequestDispatcher = TestAgent.Implementation.RequestDispatcher;

    internal static class Bootstrapper
    {
        public static void Bootstrap(
            [NotNull] Container container,
            [NotNull] string endpointRequestResponse,
            [NotNull] string endpointPublish,
            [NotNull] string sutPublishPort)
        {
            Guard.NotNull(container, nameof(container));
            Guard.NotNull(endpointRequestResponse, nameof(endpointRequestResponse));
            Guard.NotNull(endpointPublish, nameof(endpointPublish));
            Guard.NotNull(sutPublishPort, nameof(sutPublishPort));

            container.RegisterSingleton<IResolveSutExecutable, LocateSolutionConventionBasedResolveSutExecutable>();

            // sut context
            container.RegisterSingleton<IAgentContext, AgentContext>();

            // all possible request handlers
            container.Collection.Register(typeof(IRequestHandler), typeof(IRequestHandler).Assembly);
            container.Register<IRequestDispatcher, RequestDispatcher>(Lifestyle.Transient);

            BootstrapZeroMq(
                container,
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
            Guard.NotNull(container, nameof(container));
            Guard.NotNullOrWhiteSpace(endpointReqRsp, nameof(endpointReqRsp));
            Guard.NotNullOrWhiteSpace(endpointPubSub, nameof(endpointPubSub));
            Guard.NotNullOrWhiteSpace(sutPublishPort, nameof(sutPublishPort));

            // Ensures ZeroMq Context.
            container.Register<IZeroMqContextService, ZeroMqContextService>(Lifestyle.Singleton);
            container.Register<IZeroMqSocketFactory, DefaultSocketFactory>(Lifestyle.Singleton);

            container.Register<IZeroMqRequestDispatcher, ZeroMqRequestDispatcher>(Lifestyle.Transient);

            container.Register<ZeroMqPublishProxyConfig>(() => new ZeroMqPublishProxyConfig(
                new[] { endpointPubSub },
                new[] { $"tcp://*:{sutPublishPort}", FixedSettings.InternalPublishSocket },
                FixedSettings.InternalPublishProxyCapturingSocket));

            container.Register<IZeroMqPublishProxyFactory, ZeroMqPublishProxyFactory>(Lifestyle.Transient);

            container.Register<ZeroMqReqRepProxyConfig>(
                () => new ZeroMqReqRepProxyConfig(
                new[] { endpointReqRsp },
                new[] { FixedSettings.InternalRequestResponseWorkerSocket }),
                Lifestyle.Singleton);

            container.Register<IZeroMqReqRepProxyFactory, ZeroMqReqRepProxyFactory>(Lifestyle.Singleton);

            container.Register<ITestAgentEventPublisher>(
                () => new ZeroMqTestAgentEventPublisher(
                    container.GetInstance<IZeroMqContextService>(),
                    FixedSettings.InternalPublishSocket),
                Lifestyle.Singleton);
        }
    }
}
