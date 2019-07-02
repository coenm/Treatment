namespace Treatment.TestAutomation.TestRunner.Controls.Interfaces
{
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;

    internal interface ITestRunnerControlComboBox :
        IComboBox,
        IPositionUpdated,
        IIsEnabledChanged,
        ISizeUpdated,
        IFocusChange,
        IKeyboardFocusChanged,
        IDropDownOpened,
        IDropDownClosed,
        ISelectionChanged,
        IControl,
        ITestRunnerControlPositionable,
        ITestRunnerControlFocusable
    {
        string SelectedItem { get; }
    }
}
