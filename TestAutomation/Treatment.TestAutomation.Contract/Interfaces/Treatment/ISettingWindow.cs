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
    }
}
