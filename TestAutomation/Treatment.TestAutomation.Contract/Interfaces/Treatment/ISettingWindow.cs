namespace Treatment.TestAutomation.Contract.Interfaces.Treatment
{
    using global::Treatment.TestAutomation.Contract.Interfaces.Framework;
    using global::Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;

    public interface ISettingWindow :
        IInitialized,
        IWindowClosing,
        IWindowClosed,
        IWindowActivated,
        IWindowDeactivated,
        ITestAutomationView,
        IPositionUpdated,
        IIsEnabledChanged,
        ISizeUpdated,
        IFocusChange,
        IControl
    {
        IButton OpenSettingsButton { get; }

        IProjectListView ProjectList { get; }

        IMainViewStatusBar StatusBar { get; }
    }
}
