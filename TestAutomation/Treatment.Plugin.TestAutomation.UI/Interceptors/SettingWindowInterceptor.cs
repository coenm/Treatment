namespace Treatment.Plugin.TestAutomation.UI.Interceptors
{
    using System;

    using JetBrains.Annotations;
    using SimpleInjector;
    using SimpleInjector.Advanced;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.UI.View;

    internal class SettingWindowInterceptor
    {
        private readonly Container container;

        private SettingWindowInterceptor([NotNull] Container container)
        {
            Guard.NotNull(container, nameof(container));

            this.container = container;
            container.Options.RegisterResolveInterceptor(CollectResolvedMainWindowInstanceSecondWindow, c => c.Producer.ServiceType != typeof(MainWindow));
        }

        public static SettingWindowInterceptor Register(Container container) => new SettingWindowInterceptor(container);

        private object CollectResolvedMainWindowInstanceSecondWindow(InitializationContext context, Func<object> instanceProducer)
        {
            var instance = instanceProducer();

            if (instance is MainWindow)
                return instance;

            if (!(instance is SettingsWindow sw))
                return instance;

            var publisher = container.GetInstance<IEventPublisher>();
            var agent = container.GetInstance<ITestAutomationAgent>();

            var view = new SettingWindowAdapter(sw, publisher, agent);

            agent.AddPopupView(view);

            return instance;
        }
    }
}
