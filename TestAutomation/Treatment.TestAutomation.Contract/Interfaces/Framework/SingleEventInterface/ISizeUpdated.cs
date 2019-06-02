namespace Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface
{
    using System;

    using global::Treatment.TestAutomation.Contract.Interfaces.Events.Element;

    public interface ISizeUpdated
    {
        event EventHandler<SizeUpdated> SizeUpdated;
    }
}
