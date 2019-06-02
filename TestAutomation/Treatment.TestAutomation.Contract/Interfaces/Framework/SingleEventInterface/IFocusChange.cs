namespace Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface
{
    using System;

    using global::Treatment.TestAutomation.Contract.Interfaces.Events.Element;

    public interface IFocusChange
    {
        event EventHandler<FocusableChanged> FocusableChanged;

        event EventHandler<GotFocus> GotFocus;

        event EventHandler<LostFocus> LostFocus;
    }
}
