namespace Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface
{
    using System;

    using global::Treatment.TestAutomation.Contract.Interfaces.Events.Application;

    public interface IApplicationActivated
    {
         /// <summary>Occurs when an application becomes the foreground application.</summary>
         event EventHandler<ApplicationActivated> Activated;
    }
}
