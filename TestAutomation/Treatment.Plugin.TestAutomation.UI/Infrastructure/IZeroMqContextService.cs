namespace Treatment.Plugin.TestAutomation.UI.Infrastructure
{
    using ZeroMQ;

    public interface IZeroMqContextService
    {
        ZContext GetContext();

        void DisposeCurrentContext();
    }
}
