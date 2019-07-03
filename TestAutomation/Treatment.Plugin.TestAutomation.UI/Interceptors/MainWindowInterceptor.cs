namespace Treatment.Plugin.TestAutomation.UI.Interceptors
{
    using System;

    using JetBrains.Annotations;
    using SimpleInjector;
    using SimpleInjector.Advanced;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters.TreatmentControls;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.UI.Core.View;

    internal class MainWindowInterceptor
    {
        private readonly Container testAutomationContainer;

        private MainWindowInterceptor([NotNull] Container container, [NotNull] Container testAutomationContainer)
        {
            Guard.NotNull(container, nameof(container));
            Guard.NotNull(testAutomationContainer, nameof(testAutomationContainer));

            this.testAutomationContainer = testAutomationContainer;
            container.Options.RegisterResolveInterceptor(CollectResolvedMainWindowInstance, c => c.Producer.ServiceType == typeof(MainWindow));
        }

        public static MainWindowInterceptor Register([NotNull] Container container, [NotNull] Container testAutomationContainer) => new MainWindowInterceptor(container, testAutomationContainer);

        private object CollectResolvedMainWindowInstance(InitializationContext context, Func<object> instanceProducer)
        {
            var instance = instanceProducer();

            if (!(instance is MainWindow mainWindow))
                return instance;

            var publisher = testAutomationContainer.GetInstance<IEventPublisher>();
            var agent = testAutomationContainer.GetInstance<ITestAutomationAgent>();

            var view = new MainWindowAdapter(mainWindow, publisher);
            agent.RegisterAndInitializeMainView(view);

            return instance;
        }
    }
}
