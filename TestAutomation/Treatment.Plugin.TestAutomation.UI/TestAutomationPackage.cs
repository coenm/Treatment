namespace Treatment.Plugin.TestAutomation.UI
{
    using System;

    using global::TestAutomation.InputHandler.RequestHandlers;
    using JetBrains.Annotations;
    using SimpleInjector;
    using SimpleInjector.Packaging;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Interceptors;
    using Treatment.Plugin.TestAutomation.UI.Settings;
    using Treatment.Plugin.TestAutomation.UI.UserInput;
    using TreatmentZeroMq.ContextService;
    using TreatmentZeroMq.Worker;

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

            container.RegisterSingleton<IZeroMqContextService, ZeroMqContextService>();

            container.RegisterSingleton<IEventPublisher, ZeroMqEventPublisher>();
            container.RegisterSingleton<ReqRepWorkerManagement>();
            container.Register<IZeroMqRequestDispatcher, ZeroMqRequestDispatcher>(Lifestyle.Transient);

            container.RegisterSingleton<ITestAutomationAgent, TestAutomationAgent>();

            // all possible request handlers
            container.Collection.Register(typeof(IRequestHandler), typeof(IRequestHandler).Assembly);

            container.Register<IRequestDispatcher, RequestDispatcher>(Lifestyle.Transient);

            RegisterInterceptors(container);
        }

        private static void RegisterInterceptors(Container container)
        {
            ApplicationInterceptor.Register(container);
            MainWindowInterceptor.Register(container);
            SettingWindowInterceptor.Register(container);
        }
    }
}
