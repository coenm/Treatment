namespace Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface
{
    using System;

    using global::Treatment.TestAutomation.Contract.Interfaces.Events.Window;

    public interface IWindowClosed
    {
        event EventHandler<WindowClosed> WindowClosed;
    }
}
