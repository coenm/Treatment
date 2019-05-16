namespace Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface
{
    using System;

    using global::Treatment.TestAutomation.Contract.Interfaces.Events.Application;

    public interface IApplicationDeactivated
    {
        /// <summary>Occurs when an application stops being the foreground application.</summary>
        event EventHandler<ApplicationDeactivated> Deactivated;
    }
}
