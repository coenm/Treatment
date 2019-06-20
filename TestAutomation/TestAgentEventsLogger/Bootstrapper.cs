namespace TestAgentEventsLogger
{
    using CoenM.ZeroMq.ContextService;
    using CoenM.ZeroMq.Socket;
    using JetBrains.Annotations;
    using SimpleInjector;
    using Treatment.Helpers.Guards;

    internal static class Bootstrapper
    {
        public static void Bootstrap([NotNull] Container container)
        {
            Guard.NotNull(container, nameof(container));

            BootstrapZeroMq(container);
        }

        private static void BootstrapZeroMq([NotNull] Container container)
        {
            container.Register<IZeroMqContextService, ZeroMqContextService>(Lifestyle.Singleton);
            container.Register<IZeroMqSocketFactory, DefaultSocketFactory>(Lifestyle.Singleton);
        }
    }
}
