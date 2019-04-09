namespace Treatment.TestAutomation.Contract.Infrastructure
{
    using ZeroMQ;

    public interface IZeroMqContextService
    {
        ZContext GetContext();

        void DisposeCurrentContext();
    }
}
