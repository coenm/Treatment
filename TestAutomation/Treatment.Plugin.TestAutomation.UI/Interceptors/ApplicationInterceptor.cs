﻿namespace Treatment.Plugin.TestAutomation.UI.Interceptors
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using SimpleInjector;
    using SimpleInjector.Advanced;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;

    internal class ApplicationInterceptor
    {
        private readonly Container testAutomationContainer;

        private ApplicationInterceptor([NotNull] Container container, [NotNull] Container testAutomationContainer)
        {
            Guard.NotNull(container, nameof(container));
            Guard.NotNull(testAutomationContainer, nameof(testAutomationContainer));

            this.testAutomationContainer = testAutomationContainer;
            container.Options.RegisterResolveInterceptor(CollectResolvedApplication, c => c.Producer.ServiceType == typeof(Application));
        }

        public static ApplicationInterceptor Register([NotNull] Container container, [NotNull] Container testAutomationContainer) => new ApplicationInterceptor(container, testAutomationContainer);

        private object CollectResolvedApplication(InitializationContext context, Func<object> instanceProducer)
        {
            var instance = instanceProducer();

            if (!(instance is Application appInstance))
                return instance;

            var publisher = testAutomationContainer.GetInstance<IEventPublisher>();
            var agent = testAutomationContainer.GetInstance<ITestAutomationAgent>();

            var application = new ApplicationAdapter(appInstance, publisher);
            agent.RegisterAndInitializeApplication(application);

            return instance;
        }
    }
}
