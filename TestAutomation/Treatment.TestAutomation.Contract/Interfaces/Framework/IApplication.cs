namespace Treatment.TestAutomation.Contract.Interfaces.Framework
{
    using global::Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;
    using Treatment;

    public interface IApplication :
        IApplicationActivated,
        IApplicationDeactivated,
        IApplicationExit,
        IApplicationStartup,
        IControl
    {
        IMainWindow MainWindow { get; }
    }
}
