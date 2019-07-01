namespace Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface
{
    using System;

    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;

    public interface ICheckableChanged
    {
        event EventHandler<OnChecked> OnChecked;

        event EventHandler<OnUnChecked> OnUnChecked;
    }
}
