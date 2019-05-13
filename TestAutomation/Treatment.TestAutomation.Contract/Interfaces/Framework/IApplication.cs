namespace Treatment.TestAutomation.Contract.Interfaces.Framework
{
     using System;

     using Contract.Interfaces.Events.Application;

     public interface IApplication
     {
         /// <summary>Occurs when an application becomes the foreground application.</summary>
         event EventHandler Activated;

         /// <summary>Occurs when an application stops being the foreground application.</summary>
         event EventHandler Deactivated;

         /// <summary>Occurs just before an application shuts down, and cannot be canceled.</summary>
         event /*ExitEventHandler*/ EventHandler<ApplicationExit> Exit;

         /// <summary>Occurs when the <see cref="M:System.Windows.Application.Run" /> method of the <see cref="T:System.Windows.Application" /> object is called.</summary>
         event /*StartupEventHandler*/ EventHandler Startup;
     }
}
