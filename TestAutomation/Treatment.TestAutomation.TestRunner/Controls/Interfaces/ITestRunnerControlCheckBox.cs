namespace Treatment.TestAutomation.TestRunner.Controls.Interfaces
{
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;

    internal interface ITestRunnerControlCheckBox :
        ICheckBox,
        IPositionUpdated,
        IIsEnabledChanged,
        ISizeUpdated,
        IFocusChange,
        IKeyboardFocusChanged,
        ISelectionChanged,
        ICheckableChanged,
        IControl,
        ITestRunnerControlPositionable,
        ITestRunnerControlFocusable
    {
        bool IsChecked { get; }
    }
}
