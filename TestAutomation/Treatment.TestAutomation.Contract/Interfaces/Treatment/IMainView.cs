namespace Treatment.TestAutomation.Contract.Interfaces.Treatment
{
    using Framework;

    using global::Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;

    public interface IMainView :
        ITestAutomationView,
        IInitialized
    {
        IButton OpenSettingsButton { get; }

        IProjectListView ProjectList { get; }

        IMainViewStatusBar StatusBar { get; }
    }
}
