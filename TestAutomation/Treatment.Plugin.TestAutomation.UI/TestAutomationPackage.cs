namespace Treatment.Plugin.TestAutomation.UI
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using global::TestAutomation.InputHandler.RequestHandlers;
    using JetBrains.Annotations;
    using SimpleInjector;
    using SimpleInjector.Packaging;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Interceptors;
    using Treatment.Plugin.TestAutomation.UI.Settings;
    using Treatment.Plugin.TestAutomation.UI.UserInput;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.UI.View;
    using TreatmentZeroMq.ContextService;
    using TreatmentZeroMq.Worker;
    using ZeroMQ;

    using Container = SimpleInjector.Container;

    [UsedImplicitly]
    public class TestAutomationPackage : IPackage
    {
        public void RegisterServices([CanBeNull] Container container)
        {
            if (container == null)
                return;

            ITestAutomationSettings settings = new EnvironmentVariableSettings();

            if (settings.TestAutomationEnabled == false && Environment.UserInteractive)
                settings = new DummySettings();

            if (settings.TestAutomationEnabled == false)
                return;

            container.RegisterInstance<ITestAutomationSettings>(settings);
            container.RegisterSingleton<IEventPublisher, ZeroMqEventPublisher>();
            container.RegisterSingleton<IZeroMqContextService, ZeroMqContextService>();

            container.RegisterSingleton<ITestAutomationAgent, TestAutomationAgent>();

            container.RegisterSingleton<ReqRepWorkerManagement>();
            container.Register<IZeroMqRequestDispatcher, ZeroMqZeroMqRequestDispatcher>(Lifestyle.Transient);

            // all possible request handlers
            container.Collection.Register(typeof(IRequestHandler), typeof(IRequestHandler).Assembly);

            container.Register<IRequestDispatcher, RequestDispatcher>(Lifestyle.Transient);

            ApplicationInterceptor.Register(container);
            MainWindowInterceptor.Register(container);
            SettingWindowInterceptor.Register(container);
        }
 }

    internal interface ITestAutomationAgent
    {
        IApplication Application { get; }

        void AddPopupView(SettingWindowTestAutomationView view);

        void RegisterAndInitializeApplication([NotNull] IApplication application);

        void RegisterAndInitializeMainView(MainWindowTestAutomationView view);

        void RegisterWorker(Task worker);
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
        [NotNull] private readonly List<Task> workers = new List<Task>();
        private MainWindowTestAutomationView instance;
        [NotNull] private readonly ITestAutomationSettings settings;
        [NotNull] private readonly object syncLock = new object();
        [NotNull] private readonly ZContext context;
        [CanBeNull] private Task task;
        [CanBeNull] private ZSocket socket;

        private CancellationTokenSource cts;
        private MainWindowTestAutomationView view;

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

            if (view == null)
                return;

            Application.RegisterAndInitializeMainView(view);
            view = null;
        }

        public void RegisterAndInitializeMainView(MainWindowTestAutomationView view)
        {
            if (Application != null)
            {
                Application.RegisterAndInitializeMainView(view);
            }
            else
            {
                this.view = view;
            }
        }

        public void RegisterWorker(Task worker)
        {
            workers.Add(worker);
        }
    }
}
