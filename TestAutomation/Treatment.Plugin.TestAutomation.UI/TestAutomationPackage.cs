namespace Treatment.Plugin.TestAutomation.UI
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Controls;

    using JetBrains.Annotations;
    using SimpleInjector.Advanced;
    using SimpleInjector.Packaging;
    using Treatment.Helpers.Guards;
    using Treatment.UI.View;

    using Container = SimpleInjector.Container;

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

    internal class MainWindowTestAutomationView : ITestAutomationView
    {
        [NotNull] private readonly MainWindow mainWindow;

        public MainWindowTestAutomationView([NotNull] MainWindow mainWindow)
        {
            Guard.NotNull(mainWindow, nameof(mainWindow));

            this.mainWindow = mainWindow;

            mainWindow.Closing += MainWindowOnClosing;
            mainWindow.Closed += MainWindowOnClosed;

            var fields = mainWindow.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField).ToList();
            var fields1 = fields.Where(x => x.Name == nameof(OpenSettingsButton)).ToList();
            var fields2 = fields1.Where(x => x.FieldType == typeof(Button)).ToList();

            var sod = fields2.SingleOrDefault();
            if (sod != null)
                OpenSettingsButton = new ButtonAdapter((Button)sod.GetValue(mainWindow));
            else
                throw new CouldNotFindFieldException(nameof(OpenSettingsButton));
        }

        public ButtonAdapter OpenSettingsButton { get; private set; }

        public event CancelEventHandler Closing
        {
            add => mainWindow.Closing += value;
            remove => mainWindow.Closing -= value;
        }

        private void MainWindowOnClosed(object sender, EventArgs e)
        {
            mainWindow.Closing -= MainWindowOnClosing;
            mainWindow.Closed -= MainWindowOnClosed;
            return;
        }

        private void MainWindowOnClosing(object sender, CancelEventArgs e)
        {
            return;
        }
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

    internal interface ITestAutomationView
    {
    }

    public class ButtonAdapter
    {
        [NotNull] private readonly Button button;

        public ButtonAdapter([NotNull]Button button)
        {
            Guard.NotNull(button, nameof(button));
            this.button = button;
        }

        public bool IsEnabled => button.IsEnabled;

    }
}
