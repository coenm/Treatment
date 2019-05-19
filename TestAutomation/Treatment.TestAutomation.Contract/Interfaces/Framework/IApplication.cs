namespace Treatment.TestAutomation.Contract.Interfaces.Framework
{
    using global::Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;
    using JetBrains.Annotations;
    using Treatment;

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
