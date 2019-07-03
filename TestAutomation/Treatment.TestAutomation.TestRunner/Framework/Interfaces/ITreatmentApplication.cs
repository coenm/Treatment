namespace Treatment.TestAutomation.TestRunner.Framework.Interfaces
{
    using System;

    using JetBrains.Annotations;
    using Treatment.TestAutomation.Contract.Interfaces.Application;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Window;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;

    public interface ITreatmentApplication :
        IApplication,
        IApplicationActivated,
        IApplicationDeactivated,
        IApplicationExit,
        IApplicationStartup,
        IControl
    {
        event EventHandler<WindowActivated> WindowActivated;

        IMainWindow MainWindow { get; }

        [CanBeNull]
        ISettingWindow SettingsWindow { get; }

        bool Created { get; }

        ApplicationActivationState State { get; }
    }
}
