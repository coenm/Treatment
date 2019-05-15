namespace Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface
{
    using System;

    using global::Treatment.TestAutomation.Contract.Interfaces.Events.Application;

    public interface IApplicationExit
    {
        /// <summary>Occurs just before an application shuts down, and cannot be canceled.</summary>
        event /*ExitEventHandler*/ EventHandler<ApplicationExit> Exit;
    }
}