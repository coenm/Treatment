namespace Treatment.TestAutomation.Contract.Interfaces.Application
{
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;

    public interface ISettingWindow :
        IInitialized,
        IWindowClosing,
        IWindowClosed,
        IWindowActivated,
        IWindowDeactivated,
        IPositionUpdated,
        IIsEnabledChanged,
        ISizeUpdated,
        IFocusChange,
        IControl
    {
        IButton BrowseRootDirectory { get; }

        ITextBox RootDirectory { get; }

        IComboBox ComboSearchProvider { get; }

        IComboBox ComboVersionControlProvider { get; }

        ICheckBox DelayExecution { get; }

        ITextBox DelayExecutionMinValue { get; }

        ITextBox DelayExecutionMaxValue { get; }
    }
}
