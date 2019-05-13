namespace Treatment.Plugin.TestAutomation.UI
{
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Treatment.Plugin.TestAutomation.UI.Adapters;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal interface ITestAutomationAgent
    {
        IApplication Application { get; }

        void AddPopupView(SettingWindowAdapter settingWindow);

        void RegisterAndInitializeApplication([NotNull] IApplication application);

        void RegisterAndInitializeMainView(MainWindowAdapter mainWindowAdapter);

        void RegisterWorker(Task workerTask);
    }
}
