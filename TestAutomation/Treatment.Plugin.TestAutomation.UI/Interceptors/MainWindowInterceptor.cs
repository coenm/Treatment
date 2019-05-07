namespace Treatment.Plugin.TestAutomation.UI.Interceptors
{
    using System;

    using JetBrains.Annotations;
    using SimpleInjector;
    using SimpleInjector.Advanced;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces;
    using Treatment.UI.View;

    internal class MainWindowInterceptor
    {
        private readonly Container container;

        private MainWindowInterceptor([NotNull] Container container)
        {
            Guard.NotNull(container, nameof(container));
            this.container = container;
            container.Options.RegisterResolveInterceptor(CollectResolvedMainWindowInstance, c => c.Producer.ServiceType == typeof(MainWindow));
        }

        public static MainWindowInterceptor Register(Container container) => new MainWindowInterceptor(container);

        private object CollectResolvedMainWindowInstance(InitializationContext context, Func<object> instanceProducer)
        {
            var instance = instanceProducer();

            if (!(instance is MainWindow mainWindow))
                return instance;

            var publisher = container.GetInstance<IEventPublisher>();
            var agent = container.GetInstance<ITestAutomationAgent>();

            var view = new MainWindowAdapter(mainWindow, publisher);
            agent.RegisterAndInitializeMainView(view);

            return instance;
        }
    }
}
