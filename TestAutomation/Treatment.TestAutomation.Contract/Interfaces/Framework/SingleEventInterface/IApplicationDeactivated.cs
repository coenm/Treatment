namespace Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface
{
    using System;

    public interface IApplicationDeactivated
    {
        /// <summary>Occurs when an application stops being the foreground application.</summary>
        event EventHandler Deactivated;
    }
}