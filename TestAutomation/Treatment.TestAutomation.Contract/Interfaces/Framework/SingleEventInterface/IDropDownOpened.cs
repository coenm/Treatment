namespace Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface
{
    using System;

    using Events.Element;

    public interface IDropDownOpened
    {
        event EventHandler<DropDownOpened> DropDownOpened;
    }
}
