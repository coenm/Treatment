namespace Treatment.TestAutomation.Contract.Interfaces.Treatment
{
    using global::Treatment.TestAutomation.Contract.Interfaces.Framework;

    public interface ISettingWindow : ITestAutomationView
    {
        IButton OpenSettingsButton { get; }

        IProjectListView ProjectList { get; }

        IMainViewStatusBar StatusBar { get; }
    }
}
