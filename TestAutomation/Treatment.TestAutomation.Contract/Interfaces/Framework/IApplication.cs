namespace Treatment.TestAutomation.Contract.Interfaces.Framework
{
    using JetBrains.Annotations;
    using Treatment.TestAutomation.Contract.Interfaces.Application;
    using Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;

    public interface IApplication :
        IApplicationActivated,
        IApplicationDeactivated,
        IApplicationExit,
        IApplicationStartup,
        IControl
    {
        IMainWindow MainWindow { get; }

        [CanBeNull]
        ISettingWindow SettingsWindow { get; }
    }
}
