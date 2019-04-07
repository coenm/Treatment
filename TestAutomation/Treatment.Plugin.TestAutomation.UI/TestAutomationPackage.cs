namespace Treatment.Plugin.TestAutomation.UI
{
    using System;

    using JetBrains.Annotations;
    using SimpleInjector;
    using SimpleInjector.Advanced;
    using SimpleInjector.Packaging;
    using Treatment.UI.View;

    [UsedImplicitly]
    public class TestAutomationPackage : IPackage
    {
        private Container container;

        public void RegisterServices(Container container)
        {
            if (container == null)
                return;

            this.container = container;

            container.RegisterSingleton<ITestAutomationAgent, TestAutomationAgent>();

            container.Options.RegisterResolveInterceptor(CollectResolvedInstance, c => c.Producer.ServiceType == typeof(MainWindow));
        }

        private object CollectResolvedInstance(InitializationContext context, Func<object> instanceProducer)
        {
            object instance = instanceProducer();

            var agent = container.GetInstance<ITestAutomationAgent>();

            agent.RegisterView(new MainWindowTestAutomationView(instance as MainWindow));

            return instance;
        }
    }


    internal interface ITestAutomationAgent
    {
        void RegisterView(ITestAutomationView instance);
    }

    internal class TestAutomationAgent : ITestAutomationAgent
    {
        public TestAutomationAgent()
        {

        }

        public void RegisterView(ITestAutomationView instance)
        {

        }
    }

    internal class MainWindowTestAutomationView : ITestAutomationView
    {
        private readonly MainWindow mainWindow;

        public MainWindowTestAutomationView(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

        }

    }

    internal interface ITestAutomationView
    {
    }
}
