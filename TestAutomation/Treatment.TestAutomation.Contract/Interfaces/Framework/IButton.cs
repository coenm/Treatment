namespace Treatment.TestAutomation.Contract.Interfaces.Framework
{
    using Contract.Interfaces.Framework.SingleEventInterface;

    public interface IButton :
        IButtonClicked,
        IPositionUpdated,
        IIsEnabledChanged,
        ISizeUpdated,
        IControl
    {
    }
}
