namespace Treatment.TestAutomation.Contract.Interfaces.Framework
{
    using Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;

    public interface ICheckBox :
        IPositionUpdated,
        IIsEnabledChanged,
        ISizeUpdated,
        IFocusChange,
        IKeyboardFocusChanged,
        ISelectionChanged,
        ICheckableChanged,
        IControl
    {
    }
}
