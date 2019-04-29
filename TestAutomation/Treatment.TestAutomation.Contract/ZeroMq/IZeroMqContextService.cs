namespace Treatment.TestAutomation.Contract.ZeroMq
{
    using ZeroMQ;

    public interface IZeroMqContextService
    {
        ZContext GetContext();

        void DisposeCurrentContext();
    }
}
