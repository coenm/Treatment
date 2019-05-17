namespace Treatment.TestAutomation.Contract.Interfaces.Framework
{
    using global::Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;

    public interface ITextBox :
        IPositionUpdated,
        IIsEnabledChanged,
        ISizeUpdated,
        IFocusChange,
        IKeyboardFocusChanged,
        IControl
    {
    }
}
