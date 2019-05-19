namespace Treatment.TestAutomation.Contract.Interfaces.Framework
{
    using SingleEventInterface;

    public interface IComboBox :
        IPositionUpdated,
        IIsEnabledChanged,
        ISizeUpdated,
        IFocusChange,
        IKeyboardFocusChanged,
        IDropDownOpened,
        IDropDownClosed,
        ISelectionChanged,
        IControl
    {
    }
}
