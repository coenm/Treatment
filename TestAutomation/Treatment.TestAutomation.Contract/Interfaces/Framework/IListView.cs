namespace Treatment.TestAutomation.Contract.Interfaces.Framework
{
    using Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;

    public interface IListView :
        IPositionUpdated,
        ISizeUpdated,
        ILoaded,
        IControl
    {
    }
}
