namespace Treatment.Plugin.TestAutomation.UI
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Controls;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Treatment;
    using Treatment.UI.UserControls;
    using Treatment.UI.View;

    internal class MainWindowTestAutomationView : IMainView
    {
        [NotNull] private readonly MainWindow mainWindow;
        [NotNull] private readonly IEventPublisher eventPublisher;

        public MainWindowTestAutomationView([NotNull] MainWindow mainWindow, [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(mainWindow, nameof(mainWindow));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.mainWindow = mainWindow;
            this.eventPublisher = eventPublisher;

            mainWindow.Closing += MainWindowOnClosing;
            mainWindow.Closed += MainWindowOnClosed;

            var fields = mainWindow.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField).ToList();

            var fields1 = fields.Where(x => x.Name == nameof(OpenSettingsButton)).ToList();
            var fields2 = fields1.Where(x => x.FieldType == typeof(Button)).ToList();

            var sod = fields2.SingleOrDefault();
            if (sod != null)
                OpenSettingsButton = new ButtonAdapter((Button)sod.GetValue(mainWindow), eventPublisher);
            else
                throw new CouldNotFindFieldException(nameof(OpenSettingsButton));

            var fields11 = fields.Where(x => x.Name == nameof(ProjectList)).ToList();
            var fields21 = fields11.Where(x => x.FieldType == typeof(ProjectListView)).ToList();
            var item = fields21.SingleOrDefault();
            if (item != null)
                ProjectList = new ProjectListViewAdapter((ProjectListView)item.GetValue(mainWindow));
            else
                throw new CouldNotFindFieldException(nameof(ProjectList));
        }

        public IButton OpenSettingsButton { get; private set; }

        public IProjectListView ProjectList { get; private set; }

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
