namespace Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface
{
    using System;

    using global::Treatment.TestAutomation.Contract.Interfaces.Events.Application;

    public interface IApplicationStartup
    {
        /// <summary>Occurs when the <see cref="M:System.Windows.Application.Run" /> method of the <see cref="T:System.Windows.Application" /> object is called.</summary>
        event EventHandler<ApplicationStarted> Startup;
    }
}
