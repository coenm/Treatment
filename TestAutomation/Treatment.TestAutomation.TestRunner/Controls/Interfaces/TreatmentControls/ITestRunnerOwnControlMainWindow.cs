namespace Treatment.TestAutomation.TestRunner.Controls.Interfaces.TreatmentControls
{
    using Treatment.TestAutomation.Contract.Interfaces.Application;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;

    internal interface ITestRunnerOwnControlMainWindow :
        IMainWindow,
        IInitialized,
        IWindowClosing,
        IWindowClosed,
        IWindowActivated,
        IWindowDeactivated,
        IPositionUpdated,
        IIsEnabledChanged,
        ISizeUpdated,
        IFocusChange,
        IControl,
        IElementLoadedUnLoaded,
        ITestRunnerControlPositionable,
        ITestRunnerControlFocusable
    {
    }
}
