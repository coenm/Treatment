namespace Treatment.TestAutomation.Contract.Interfaces.Framework
{
    using Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;

    public interface ITextBlock :
        IPositionUpdated,
        ISizeUpdated,
        IIsEnabledChanged,
        ITextValueChanged,
        IControl
    {
    }
}
