namespace Treatment.TestAutomation.Contract.Interfaces.Framework
{
    using Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;

    public interface IComboBox :
        IPositionUpdated,
        IIsEnabledChanged,
        ISizeUpdated,
        IFocusChange,
        IKeyboardFocusChanged,
        IDropDownOpened,
        IDropDownClosed,
        ISelectionChanged,
        IControl,
        IElementLoadedUnLoaded
    {
    }
}
