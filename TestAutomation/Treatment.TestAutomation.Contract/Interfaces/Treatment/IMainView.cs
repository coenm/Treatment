namespace Treatment.TestAutomation.Contract.Interfaces.Treatment
{
    using Framework;

    public interface ITreatmentTestAutomationApplicationContext : ITestAutomationApplicationContext
    {

    }

    public interface IMainView : ITestAutomationView
    {
        IButton OpenSettingsButton { get; }

        IProjectListView ProjectList { get; }
    }
}
