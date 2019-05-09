namespace Treatment.TestAutomation.Contract.Interfaces.Framework
{
    using JetBrains.Annotations;

    public interface ITestAutomationApplicationContext
    {
        [CanBeNull]
        ITestAutomationView MainView { get; }
    }
}
