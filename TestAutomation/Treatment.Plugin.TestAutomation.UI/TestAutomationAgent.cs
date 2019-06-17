namespace Treatment.Plugin.TestAutomation.UI
{
    using System.Threading;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters;
    using Treatment.Plugin.TestAutomation.UI.Interfaces;
    using Treatment.Plugin.TestAutomation.UI.Settings;

    internal class TestAutomationAgent : ITestAutomationAgent
    {
        [NotNull] private readonly ITestAutomationSettings settings;
        [NotNull] private readonly object syncLock = new object();
        [NotNull] private readonly CancellationTokenSource cts = new CancellationTokenSource();
        [CanBeNull] private MainWindowAdapter mainWindow;
        [CanBeNull] private SettingWindowAdapter settingWindow;
        [CanBeNull] private ITestAutomationApplication application;

        public TestAutomationAgent([NotNull] ITestAutomationSettings settings)
        {
            Guard.NotNull(settings, nameof(settings));
            this.settings = settings;
        }

        public void RegisterAndInitializeApplication([NotNull] ITestAutomationApplication application)
        {
            Guard.NotNull(application, nameof(application));
            this.application = application;
            this.application.Initialize();

            if (mainWindow == null)
                return;

            this.application.RegisterAndInitializeMainView(mainWindow);
        }

        public void RegisterAndInitializeMainView([NotNull] MainWindowAdapter mainWindowAdapter)
        {
            Guard.NotNull(mainWindowAdapter, nameof(mainWindowAdapter));
            application?.RegisterAndInitializeMainView(mainWindowAdapter);

            mainWindow = mainWindowAdapter;
        }

        public void AddPopupViewAndInitialize([NotNull] SettingWindowAdapter settingWindow)
        {
            // todo: wrong method name. how to deal with this popup window?
            Guard.NotNull(settingWindow, nameof(settingWindow));

            application?.RegisterAndInitializeSettings(settingWindow);
            this.settingWindow = settingWindow;
            // settingWindow.Initialize();
        }
    }
}
