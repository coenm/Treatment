namespace Treatment.Plugin.TestAutomation.UI
{
    using System;
    using JetBrains.Annotations;
    using SimpleInjector.Advanced;
    using SimpleInjector.Packaging;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.UI.View;

    using Container = SimpleInjector.Container;

    [UsedImplicitly]
    public class TestAutomationPackage : IPackage
    {
        private Container container;

        public void RegisterServices([CanBeNull] Container container)
        {
            if (container == null)
                return;

            this.container = container;

            container.RegisterSingleton<ITestAutomationAgent, TestAutomationAgent>();

            container.Options.RegisterResolveInterceptor(CollectResolvedMainWindowInstance, c => c.Producer.ServiceType == typeof(MainWindow));
        }

        private object CollectResolvedMainWindowInstance(InitializationContext context, Func<object> instanceProducer)
        {
            var instance = instanceProducer();

            if (!(instance is MainWindow mainWindow))
                return instance;

            var agent = container.GetInstance<ITestAutomationAgent>();
            agent.RegisterMainView(new MainWindowTestAutomationView(mainWindow));

            return instance;
        }
    }

    internal interface ITestAutomationAgent
    {
        void RegisterMainView([NotNull] ITestAutomationView instance);
    }

    [Serializable]
    internal class CouldNotFindFieldException : Exception
    {
        public CouldNotFindFieldException(string fieldName)
        {}
    }

    internal class TestAutomationAgent : ITestAutomationAgent
    {
        public TestAutomationAgent()
        {

        }

        public void RegisterMainView([NotNull] ITestAutomationView instance)
        {
            Guard.NotNull(instance, nameof(instance));
        }
    }
}
