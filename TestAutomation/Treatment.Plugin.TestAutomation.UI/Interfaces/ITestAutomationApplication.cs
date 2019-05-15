namespace Treatment.Plugin.TestAutomation.UI.Interfaces
{
    using JetBrains.Annotations;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Treatment;

    internal interface ITestAutomationApplication : IApplication, IUiElement
    {
        IMainView MainView { get; }

        void RegisterAndInitializeMainView([NotNull] IMainView mainView);
    }
}
