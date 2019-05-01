namespace TestAgentEventsLogger
{
    using JetBrains.Annotations;
    using SimpleInjector;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.EventSerializers;
    using Treatment.TestAutomation.Contract.ZeroMq;

    internal static class Bootstrapper
    {
        public static void Bootstrap([NotNull] Container container)
        {
            Guard.NotNull(container, nameof(container));

            // Events from the TestAutomation plugin (specified in TestAutomation contact).
            container.Collection.Register(typeof(IEventSerializer), typeof(IEventSerializer).Assembly);

            BootstrapZeroMq(container);
        }

        private static void BootstrapZeroMq([NotNull] Container container)
        {
            // Ensures ZeroMq Context.
            container.RegisterSingleton<IZeroMqContextService, ZeroMqContextService>();
        }
    }
}
