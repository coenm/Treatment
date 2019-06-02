namespace Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface
{
    using System;

    using global::Treatment.TestAutomation.Contract.Interfaces.Events.ButtonBase;

    public interface IButtonClicked
    {
        event EventHandler<Clicked> Clicked;
    }
}
