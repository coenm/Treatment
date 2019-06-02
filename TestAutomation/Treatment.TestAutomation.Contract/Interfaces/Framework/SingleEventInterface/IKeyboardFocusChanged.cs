namespace Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface
{
    using System;

    using global::Treatment.TestAutomation.Contract.Interfaces.Events.Element;

    public interface IKeyboardFocusChanged
    {
        /// <summary>Occurs when an application becomes the foreground application.</summary>
        event EventHandler<KeyboardFocusChanged> KeyboardFocusChanged;
    }
}
