namespace Treatment.Plugin.TestAutomation.UI.Infrastructure.Infrastructure
{
    using ZeroMQ;

    public interface IZeroMqContextService
    {
        ZContext GetContext();

        void DisposeCurrentContext();
    }
}
