namespace Treatment.Plugin.TestAutomation.UI
{
    using System;

    using JetBrains.Annotations;
    using SimpleInjector;
    using SimpleInjector.Packaging;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Interceptors;
    using Treatment.Plugin.TestAutomation.UI.Settings;
    using TreatmentZeroMq.ContextService;

    [UsedImplicitly]
    public class TestAutomationPackage : IPackage
    {
        private Container testAutomationContainer;
        private ITestAutomationAgent agent;

        public void RegisterServices([CanBeNull] Container container)
        {
            if (container == null)
                return;

            ITestAutomationSettings settings = new EnvironmentVariableSettings();

            if (settings.TestAutomationEnabled == false && Environment.UserInteractive)
                settings = new DummySettings();

            if (settings.TestAutomationEnabled == false)
                return;

            testAutomationContainer = new Container();

            testAutomationContainer.RegisterInstance(settings);

            testAutomationContainer.RegisterSingleton<IZeroMqContextService, ZeroMqContextService>();

            testAutomationContainer.RegisterSingleton<IEventPublisher, ZeroMqEventPublisher>();

            testAutomationContainer.RegisterSingleton<ITestAutomationAgent, TestAutomationAgent>();

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
