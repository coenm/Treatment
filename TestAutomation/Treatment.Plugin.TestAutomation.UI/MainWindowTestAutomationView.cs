namespace Treatment.Plugin.TestAutomation.UI
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Controls;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract;
    using Treatment.UI.View;

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

        public IButton OpenSettingsButton { get; private set; }

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
