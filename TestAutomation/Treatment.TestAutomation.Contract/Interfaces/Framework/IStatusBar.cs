namespace Treatment.TestAutomation.Contract.Interfaces.Framework
{
    using Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;

    public interface IStatusBar :
        IPositionUpdated,
        ISizeUpdated,
        ILoaded,
        IControl
    {
    }
}
