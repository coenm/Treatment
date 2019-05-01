namespace TestAgentEventsLogger
{
    using JetBrains.Annotations;
    using SimpleInjector;
    using Treatment.Helpers.Guards;
    using TreatmentZeroMq;
    using TreatmentZeroMq.ContextService;

    internal static class Bootstrapper
    {
        public static void Bootstrap([NotNull] Container container)
        {
            Guard.NotNull(container, nameof(container));

            BootstrapZeroMq(container);
        }

        private static void BootstrapZeroMq([NotNull] Container container)
        {
            // Ensures ZeroMq Context.
            container.RegisterSingleton<IZeroMqContextService, ZeroMqContextService>();
        }
    }
}
