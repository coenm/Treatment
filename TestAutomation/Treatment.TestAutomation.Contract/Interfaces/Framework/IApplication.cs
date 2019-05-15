namespace Treatment.TestAutomation.Contract.Interfaces.Framework
{
    using global::Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;

    public interface IApplication :
        IApplicationActivated,
        IApplicationDeactivated,
        IApplicationExit,
        IApplicationStartup,
        IControl
    {
    }
}
