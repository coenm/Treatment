namespace Treatment.Plugin.TestAutomation.UI.Interfaces
{
    using JetBrains.Annotations;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Treatment;

    internal interface ITestAutomationApplication : IApplication, IUiElement
    {
        IMainWindow MainWindow { get; }

        void RegisterAndInitializeMainView([NotNull] IMainWindow mainWindow);
    }
}
