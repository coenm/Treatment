namespace Treatment.Plugin.TestAutomation.UI.Interceptors
{
    using System;

    using JetBrains.Annotations;
    using SimpleInjector;
    using SimpleInjector.Advanced;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters;
    using Treatment.Plugin.TestAutomation.UI.Adapters.TreatmentControls;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.UI.Core.View;

    internal class SettingWindowInterceptor
    {
        private readonly Container testAutomationContainer;

        private SettingWindowInterceptor([NotNull] Container container, [NotNull] Container testAutomationContainer)
        {
            Guard.NotNull(container, nameof(container));
            Guard.NotNull(testAutomationContainer, nameof(testAutomationContainer));

            this.testAutomationContainer = testAutomationContainer;
            container.Options.RegisterResolveInterceptor(CollectResolvedMainWindowInstanceSecondWindow, c => c.Producer.ServiceType != typeof(MainWindow));
        }

        public static SettingWindowInterceptor Register([NotNull] Container container, [NotNull] Container testAutomationContainer) => new SettingWindowInterceptor(container, testAutomationContainer);

        private object CollectResolvedMainWindowInstanceSecondWindow(InitializationContext context, Func<object> instanceProducer)
        {
            var instance = instanceProducer();

            if (instance is MainWindow)
                return instance;

            if (!(instance is SettingsWindow sw))
                return instance;

            var publisher = testAutomationContainer.GetInstance<IEventPublisher>();
            var agent = testAutomationContainer.GetInstance<ITestAutomationAgent>();

            var view = new SettingWindowAdapter(sw, publisher);
            agent.AddPopupViewAndInitialize(view);

            return instance;
        }
    }
}
