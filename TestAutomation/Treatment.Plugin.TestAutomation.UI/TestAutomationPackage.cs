namespace Treatment.Plugin.TestAutomation.UI
{
    using System;
    using System.Threading;

    using JetBrains.Annotations;
    using SimpleInjector.Advanced;
    using SimpleInjector.Packaging;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Settings;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.UI.View;

    using Container = SimpleInjector.Container;

    [UsedImplicitly]
    public class TestAutomationPackage : IPackage
    {
        private Container container;

        public void RegisterServices([CanBeNull] Container container)
        {
            if (container == null)
                return;

            ITestAutomationSettings settings = new EnvironmentVariableSettings();

            if (settings.TestAutomationEnabled == false && Environment.UserInteractive)
                settings = new DummySettings();

            if (settings.TestAutomationEnabled == false)
                return;

            this.container = container;

            container.RegisterInstance<ITestAutomationSettings>(settings);
            container.RegisterSingleton<IEventPublisher, ZeroMqEventPublisher>();
            container.RegisterSingleton<IZeroMqContextService, ZeroMqContextService>();
            container.RegisterSingleton<ITestAutomationEndpoint, ZeroMqEndpoint>();

            container.RegisterSingleton<ITestAutomationAgent, TestAutomationAgent>();

            container.Options.RegisterResolveInterceptor(CollectResolvedMainWindowInstance, c => c.Producer.ServiceType == typeof(MainWindow));
        }

        private object CollectResolvedMainWindowInstance(InitializationContext context, Func<object> instanceProducer)
        {
            var instance = instanceProducer();

            if (!(instance is MainWindow mainWindow))
                return instance;

            var publisher = container.GetInstance<IEventPublisher>();
            var agent = container.GetInstance<ITestAutomationAgent>();

            for (var i = 3; i >= 0; i--)
            {
                publisher.PublishAsync(new TestAutomationEvent
                {
                    Control = $"Start in {i} seconds",
                });

                if (i > 0)
                    Thread.Sleep(1000);
            }

            agent.RegisterMainView(new MainWindowTestAutomationView(mainWindow, publisher));

            return instance;
        }
    }

    internal interface ITestAutomationAgent
    {
        void RegisterMainView([NotNull] ITestAutomationView instance);
    }

    internal class TestAutomationAgent : ITestAutomationAgent
    {
        [NotNull] private readonly ITestAutomationEndpoint endpoint;

        public TestAutomationAgent([NotNull] ITestAutomationEndpoint endpoint)
        {
            Guard.NotNull(endpoint, nameof(endpoint));
            this.endpoint = endpoint;
        }

        public void Start()
        {
            endpoint.StartAccepting();
        }

        public void RegisterMainView([NotNull] ITestAutomationView instance)
        {
            Guard.NotNull(instance, nameof(instance));
        }
    }
}
