namespace Treatment.Plugin.TestAutomation.UI
{
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters;
    using Treatment.Plugin.TestAutomation.UI.Settings;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class TestAutomationAgent : ITestAutomationAgent
    {
        [NotNull] private readonly ITestAutomationSettings settings;
        [NotNull] private readonly object syncLock = new object();
        [CanBeNull] private Task worker;
        [CanBeNull] private MainWindowAdapter mainWindow;
        [CanBeNull] private SettingWindowAdapter settingWindow;

        [NotNull] private readonly CancellationTokenSource cts = new CancellationTokenSource();

        public TestAutomationAgent([NotNull] ITestAutomationSettings settings)
        {
            Guard.NotNull(settings, nameof(settings));
            this.settings = settings;
        }

        public IApplication Application { get; private set; }

        public void RegisterAndInitializeApplication([NotNull] IApplication application)
        {
            Guard.NotNull(application, nameof(application));
            Application = application;
            Application.Initialize();

            if (mainWindow == null)
                return;

            Application.RegisterAndInitializeMainView(mainWindow);
        }

        public void RegisterAndInitializeMainView([NotNull] MainWindowAdapter mainWindowAdapter)
        {
            Guard.NotNull(mainWindowAdapter, nameof(mainWindowAdapter));
            Application?.RegisterAndInitializeMainView(mainWindowAdapter);

            mainWindow = mainWindowAdapter;
        }

        public void AddPopupView([NotNull] SettingWindowAdapter settingWindow)
        {
            // todo: wrong method name. how to deal with this popup window?
            Guard.NotNull(settingWindow, nameof(settingWindow));
            this.settingWindow = settingWindow;
            settingWindow.Initialize();
        }

        public void RegisterWorker([NotNull] Task task)
        {
            Guard.NotNull(task, nameof(task));

            worker = task;
        }
    }
}
