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
        public static void Bootstrap([NotNull] Container container)
        {
            Guard.NotNull(container, nameof(container));

            // sut context
            container.RegisterSingleton<ISutContext, SutContext>();

            // Events from the TestAutomation plugin (specified in TestAutomation contact).
            container.Collection.Register(typeof(IEventSerializer), typeof(IEventSerializer).Assembly);

            // all possible request handlers
            container.Collection.Register(typeof(IRequestHandler), typeof(IRequestHandler).Assembly);

            container.Register<IRequestDispatcher, RequestDispatcher>(Lifestyle.Transient);


            container.Register<ILogger, EmptyLogger>(Lifestyle.Singleton);

            BootstrapZeroMq(container);
        }

        private static void BootstrapZeroMq([NotNull] Container container)
        {
            // Ensures ZeroMq Context.
            container.RegisterSingleton<IZeroMqContextService, ZeroMqContextService>();
            container.Register<IZeroMqRequestDispatcher, ZeroMqZeroMqRequestDispatcher>(Lifestyle.Transient);

            container.Register<ZeroMqPublishProxyConfig>(() => new ZeroMqPublishProxyConfig(string.Empty, string.Empty));
            container.Register<IZeroMqPublishProxyFactory, ZeroMqPublishProxyFactory>(Lifestyle.Transient);

            container.Register<ZeroMqReqRepProxyConfig>(() => new ZeroMqReqRepProxyConfig(string.Empty, string.Empty));
            container.Register<IZeroMqReqRepProxyFactory, ZeroMqReqRepProxyFactory>(Lifestyle.Transient);
        }
    }
}
