namespace Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface
{
    using System;
    using Events.Element;

    public interface IDropDownClosed
    {
        event EventHandler<DropDownClosed> DropDownClosed;
    }
}
