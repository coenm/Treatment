namespace Treatment.Plugin.TestAutomation.UI.Interfaces
{
    using JetBrains.Annotations;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal interface ITestAutomationApplication : IApplication, IGuid, IUiElement
    {
        void RegisterAndInitializeMainView([NotNull] ITestAutomationMainWindow mainWindow);
    }
}
