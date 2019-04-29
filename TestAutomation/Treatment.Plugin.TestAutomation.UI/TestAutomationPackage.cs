﻿namespace Treatment.Plugin.TestAutomation.UI
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    using JetBrains.Annotations;
    using SimpleInjector.Advanced;
    using SimpleInjector.Packaging;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Settings;
    using Treatment.TestAutomation.Contract.Interfaces.EventSerializers;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
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

            container.Collection.Register(typeof(IEventSerializer), typeof(IEventSerializer).Assembly);

            container.Options.RegisterResolveInterceptor(CollectResolvedApplication, c => c.Producer.ServiceType == typeof(Application));

            container.Options.RegisterResolveInterceptor(CollectResolvedMainWindowInstance, c => c.Producer.ServiceType == typeof(MainWindow));

            container.Options.RegisterResolveInterceptor(CollectResolvedMainWindowInstanceSecondWindow, c =>
            {
                if (c.Producer.ServiceType == typeof(MainWindow))
                    return false;
                return true;
            });
        }

        private object CollectResolvedApplication(InitializationContext context, Func<object> instanceProducer)
        {
            var instance = instanceProducer();

            if (!(instance is Application appInstance))
                return instance;

            var publisher = container.GetInstance<IEventPublisher>();
            var agent = container.GetInstance<ITestAutomationAgent>();

            var applicationAdapter = new ApplicationAdapter(appInstance, publisher);
            agent.RegisterAndInitializeApplication(applicationAdapter);

            return instance;
        }

        private object CollectResolvedMainWindowInstance(InitializationContext context, Func<object> instanceProducer)
        {
            var instance = instanceProducer();

            if (!(instance is MainWindow mainWindow))
                return instance;

            var publisher = container.GetInstance<IEventPublisher>();
            var agent = container.GetInstance<ITestAutomationAgent>();

            var view = new MainWindowTestAutomationView(mainWindow, publisher);
            agent.Application.RegisterAndInitializeMainView(view);

            publisher.PublishAsync(new TestAutomationEvent
            {
                Control = null,
                EventName = "Creation",
                Payload = view.Guid,
            });

            return instance;
        }

        private object CollectResolvedMainWindowInstanceSecondWindow(InitializationContext context, Func<object> instanceProducer)
        {
            var instance = instanceProducer();

            if (instance is MainWindow)
                return instance;

            if (!(instance is SettingsWindow sw))
                return instance;

            var publisher = container.GetInstance<IEventPublisher>();
            var agent = container.GetInstance<ITestAutomationAgent>();

            var view = new SettingWindowTestAutomationView(sw, publisher, agent);
            publisher.PublishAsync(new TestAutomationEvent
            {
                Control = null,
                EventName = "Creation",
                Payload = view.Guid,
            });

            agent.AddPopupView(view);

            return instance;
        }
    }

    internal interface ITestAutomationAgent
    {
        IApplication Application { get; }

        void AddPopupView(SettingWindowTestAutomationView view);

        void RegisterAndInitializeApplication([NotNull] IApplication application);
    }

    internal class SettingWindowTestAutomationView : ITestAutomationView
    {
        [NotNull] private SettingsWindow settingsWindow;
        [NotNull] private IEventPublisher publisher;
        [NotNull] private ITestAutomationAgent agent;

        public SettingWindowTestAutomationView([NotNull] SettingsWindow settingsWindow, [NotNull] IEventPublisher publisher, [NotNull] ITestAutomationAgent agent)
        {
            this.settingsWindow = settingsWindow ?? throw new ArgumentNullException(nameof(settingsWindow));
            this.publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            this.agent = agent ?? throw new ArgumentNullException(nameof(agent));

            Guid = Guid.NewGuid();
        }

        public Guid Guid { get; }

        public void Dispose()
        {
        }

        public void Initialize()
        {
        }
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

        public IApplication Application { get; private set; }


        public void AddPopupView(SettingWindowTestAutomationView view)
        {
            view.Initialize();
        }

        public void RegisterAndInitializeApplication([NotNull] IApplication application)
        {
            Guard.NotNull(application, nameof(application));
            Application = application;
            Application.Initialize();
        }
    }
}
