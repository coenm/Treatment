namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;
    using System.ComponentModel;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Reflection;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Treatment;
    using Treatment.UI.UserControls;
    using Treatment.UI.View;

    internal class MainWindowTestAutomationView : IMainView
    {
        [NotNull] private readonly MainWindow mainWindow;
        [NotNull] private readonly IEventPublisher eventPublisher;
        [NotNull] private readonly ITestAutomationAgent agent;

        public MainWindowTestAutomationView([NotNull] MainWindow mainWindow, [NotNull] IEventPublisher eventPublisher, [NotNull] ITestAutomationAgent agent)
        {
            Guard.NotNull(mainWindow, nameof(mainWindow));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));
            Guard.NotNull(agent, nameof(agent));

            this.mainWindow = mainWindow;
            this.eventPublisher = eventPublisher;
            this.agent = agent;

            mainWindow.Closing += MainWindowOnClosing;
            mainWindow.Closed += MainWindowOnClosed;

            mainWindow.Closed += (sender, args) => agent.Stop();

            OpenSettingsButton = new ButtonAdapter(
                FieldsHelper.FindFieldInUiElementByName<Button>(mainWindow, nameof(OpenSettingsButton)),
                eventPublisher);

            StatusBar = new MainViewStatusBarAdapter(
                FieldsHelper.FindFieldInUiElementByName<StatusBar>(mainWindow, nameof(StatusBar)),
                eventPublisher);

            ProjectList = new ProjectListViewAdapter(
                FieldsHelper.FindFieldInUiElementByName<ProjectListView>(mainWindow, nameof(ProjectList)),
                eventPublisher);
        }

        public IButton OpenSettingsButton { get; private set; }

        public IProjectListView ProjectList { get; private set; }

        public IMainViewStatusBar StatusBar { get; private set; }

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
}
