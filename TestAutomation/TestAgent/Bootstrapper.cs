﻿namespace TestAgent
{
    using System.Collections.Generic;

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
            [NotNull] string sutPublishPort,
            [NotNull] string sutReqRspPort)
        {
            Guard.NotNull(container, nameof(container));
            Guard.NotNull(endpointRequestResponse, nameof(endpointRequestResponse));
            Guard.NotNull(endpointPublish, nameof(endpointPublish));
            Guard.NotNull(sutPublishPort, nameof(sutPublishPort));
            Guard.NotNull(sutReqRspPort, nameof(sutReqRspPort));

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
                sutPublishPort,
                sutReqRspPort);
        }

        private static void BootstrapZeroMq(
            [NotNull] Container container,
            [NotNull] string endpointReqRsp,
            [NotNull] string endpointPubSub,
            [NotNull] string sutPublishPort,
            [NotNull] string sutReqRspPort)
        {
            Guard.NotNull(container, nameof(container));
            Guard.NotNullOrWhiteSpace(endpointReqRsp, nameof(endpointReqRsp));
            Guard.NotNullOrWhiteSpace(endpointPubSub, nameof(endpointPubSub));
            Guard.NotNullOrWhiteSpace(sutPublishPort, nameof(sutPublishPort));
            Guard.NotNullOrWhiteSpace(sutReqRspPort, nameof(sutReqRspPort));

            // Ensures ZeroMq Context.
            container.Register<IZeroMqContextService, ZeroMqContextService>(Lifestyle.Singleton);
            container.Register<IZeroMqSocketFactory, DefaultSocketFactory>(Lifestyle.Singleton);

            container.Register<IZeroMqRequestDispatcher, ZeroMqRequestDispatcher>(Lifestyle.Transient);

            container.Register<ZeroMqPublishProxyConfig>(() => new ZeroMqPublishProxyConfig(
                new[] { endpointPubSub },
                new[] { $"tcp://*:{sutPublishPort}", "inproc://publish" },
                "inproc://capturePubSub"));

            container.Register<IZeroMqPublishProxyFactory, ZeroMqPublishProxyFactory>(Lifestyle.Transient);

            container.Register<ZeroMqReqRepProxyConfig>(
                () => new ZeroMqReqRepProxyConfig(
                new[] { endpointReqRsp },
                new Dictionary<string, string>
                {
                    { "TESTAGENT", "inproc://reqrsp" },
                }),
                Lifestyle.Singleton);

            container.Register<IZeroMqReqRepProxyFactory, ZeroMqReqRepProxyFactory>(Lifestyle.Singleton);
        }
    }
}
