namespace Treatment.TestAutomation.Contract.Interfaces.Treatment
{
    using Framework;

    public interface IMainView : ITestAutomationView
    {
//        ITestAutomationButton OpenSettingsButton { get; }

        IProjectListView ProjectList { get; }

        IMainViewStatusBar StatusBar { get; }
    }
}
