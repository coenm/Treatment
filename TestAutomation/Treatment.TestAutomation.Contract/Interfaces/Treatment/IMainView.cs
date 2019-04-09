namespace Treatment.TestAutomation.Contract.Interfaces.Treatment
{
    using Framework;

    public interface IMainView : ITestAutomationView
    {
        IButton OpenSettingsButton { get; }
    }
}
