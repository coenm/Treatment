namespace Treatment.Plugin.TestAutomation.UI.Interceptors
{
    using System;
    using System.Threading;
    using System.Windows;

    using JetBrains.Annotations;

    using SimpleInjector;
    using SimpleInjector.Advanced;

    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Settings;

    using TreatmentZeroMq.Worker;

    internal class ApplicationInterceptor
    {
        private readonly Container container;

        private ApplicationInterceptor([NotNull] Container container)
        {
            Guard.NotNull(container, nameof(container));
            this.container = container;
            container.Options.RegisterResolveInterceptor(CollectResolvedApplication, c => c.Producer.ServiceType == typeof(Application));
        }

        public static ApplicationInterceptor Register(Container container) => new ApplicationInterceptor(container);

        private object CollectResolvedApplication(InitializationContext context, Func<object> instanceProducer)
        {
            var instance = instanceProducer();

            if (!(instance is Application appInstance))
                return instance;

            var publisher = container.GetInstance<IEventPublisher>();
            var agent = container.GetInstance<ITestAutomationAgent>();

            var applicationAdapter = new ApplicationAdapter(appInstance, publisher);
            agent.RegisterAndInitializeApplication(applicationAdapter);

            var config = container.GetInstance<ITestAutomationSettings>();

            if (string.IsNullOrWhiteSpace(config.ZeroMqRequestResponseSocket))
                return instance;

            StartAndRegisterRequestResponseWorker(config, agent);

            return instance;
        }

        private void StartAndRegisterRequestResponseWorker(ITestAutomationSettings config, ITestAutomationAgent agent)
        {
            var worker = container.GetInstance<ReqRepWorkerManagement>()
                                  .StartSingleWorker(
                                                     container.GetInstance<IZeroMqRequestDispatcher>(),
                                                     config.ZeroMqRequestResponseSocket,
                                                     CancellationToken.None);
            agent.RegisterWorker(worker);
        }
    }
}
