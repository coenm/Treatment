namespace Treatment.Plugin.TestAutomation.UI
{
    using System;

    using global::TestAutomation.InputHandler.RequestHandlers;
    using JetBrains.Annotations;
    using NLog;
    using SimpleInjector;
    using SimpleInjector.Packaging;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Interceptors;
    using Treatment.Plugin.TestAutomation.UI.Settings;
    using Treatment.Plugin.TestAutomation.UI.UserInput;
    using TreatmentZeroMq.ContextService;
    using TreatmentZeroMq.Worker;

    [UsedImplicitly]
    public class TestAutomationPackage : IPackage
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private Container testAutomationContainer;
        private ITestAutomationAgent agent;

        public void RegisterServices([CanBeNull] Container container)
        {
            if (container == null)
                return;

            try
            {
                RegisterServicesImpl(container);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Could not register TestAutomation module: {e.Message}");
                Logger.Debug(e.StackTrace);
            }
        }

        private void RegisterServicesImpl([NotNull] Container container)
        {
            ITestAutomationSettings settings = new EnvironmentVariableSettings();

            if (settings.TestAutomationEnabled == false && Environment.UserInteractive)
                settings = new DummySettings();

            if (settings.TestAutomationEnabled == false)
                return;

            testAutomationContainer = new Container();

            testAutomationContainer.RegisterInstance(settings);

            testAutomationContainer.RegisterSingleton<IZeroMqContextService, ZeroMqContextService>();

            testAutomationContainer.RegisterSingleton<IEventPublisher, ZeroMqEventPublisher>();
            testAutomationContainer.RegisterSingleton<ReqRepWorkerManagement>();
            testAutomationContainer.Register<IZeroMqRequestDispatcher, ZeroMqRequestDispatcher>(Lifestyle.Transient);

            testAutomationContainer.RegisterSingleton<ITestAutomationAgent, TestAutomationAgent>();

            // all possible request handlers
            testAutomationContainer.Collection.Register(typeof(IRequestHandler), typeof(IRequestHandler).Assembly);

            testAutomationContainer.Register<IRequestDispatcher, RequestDispatcher>(Lifestyle.Transient);

            RegisterInterceptors(container, testAutomationContainer);

            agent = testAutomationContainer.GetInstance<ITestAutomationAgent>();
        }

        private static void RegisterInterceptors(Container container, Container testAutomationContainer)
        {
            DebugGuard.NotNull(container, nameof(container));
            DebugGuard.NotNull(testAutomationContainer, nameof(testAutomationContainer));

            ApplicationInterceptor.Register(container, testAutomationContainer);
            MainWindowInterceptor.Register(container, testAutomationContainer);
            SettingWindowInterceptor.Register(container, testAutomationContainer);
        }
    }
}
