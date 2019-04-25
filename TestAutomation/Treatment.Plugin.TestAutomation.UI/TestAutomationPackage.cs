namespace Treatment.Plugin.TestAutomation.UI
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using SimpleInjector.Advanced;
    using SimpleInjector.Packaging;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Settings;
    using Treatment.UI.View;
    using ZeroMQ;

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

            var view = new MainWindowTestAutomationView(mainWindow, publisher, agent);
            publisher.PublishAsync(new TestAutomationEvent
            {
                Control = null,
                EventName = "Creation",
                Payload = view.Guid,
            });

            agent.RegisterMainView(view);

            return instance;
        }
    }

    internal interface ITestAutomationAgent
    {
        void RegisterMainView([NotNull] MainWindowTestAutomationView instance);

        void Stop();
    }

    internal class TestAutomationAgent : ITestAutomationAgent
    {
        private MainWindowTestAutomationView instance;
        [NotNull] private readonly ITestAutomationSettings settings;
        [NotNull] private readonly object syncLock = new object();
        [NotNull] private readonly ZContext context;
        [CanBeNull] private Task task;
        [CanBeNull] private ZSocket socket;

        private CancellationTokenSource cts;

        public TestAutomationAgent([NotNull] IZeroMqContextService contextService, [NotNull] ITestAutomationSettings settings)
        {
            Guard.NotNull(contextService, nameof(contextService));
            Guard.NotNull(settings, nameof(settings));

            this.settings = settings;
            context = contextService.GetContext() ?? throw new NullReferenceException();
        }

        public void Stop()
        {
            cts?.Cancel();
            instance = null;
        }

        public void RegisterMainView([NotNull] MainWindowTestAutomationView instance)
        {
            Guard.NotNull(instance, nameof(instance));
            this.instance = instance;
            instance.Initialize();
        }
    }
}
