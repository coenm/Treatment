namespace Treatment.Plugin.TestAutomation.UI
{
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Treatment.Plugin.TestAutomation.UI.Adapters;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal interface ITestAutomationAgent
    {
        IApplication Application { get; }

        void AddPopupView(SettingWindowAdapter view);

        void RegisterAndInitializeApplication([NotNull] IApplication application);

        void RegisterAndInitializeMainView(MainWindowTestAutomationView view);

        void RegisterWorker(Task worker);
    }
}
