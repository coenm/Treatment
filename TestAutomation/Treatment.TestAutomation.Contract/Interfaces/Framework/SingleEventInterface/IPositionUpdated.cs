namespace Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface
{
    using System;

    using global::Treatment.TestAutomation.Contract.Interfaces.Events.Element;

    public interface IPositionUpdated
    {
        event EventHandler<PositionUpdated> PositionUpdated;
    }
}
