namespace Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface
{
    using System;

    public interface IApplicationActivated
    {
         /// <summary>Occurs when an application becomes the foreground application.</summary>
         event EventHandler Activated;
    }
}
