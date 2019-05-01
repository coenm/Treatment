namespace TreatmentZeroMq.ContextService
{
    using ZeroMQ;

    public interface IZeroMqContextService
    {
        ZContext GetContext();

        void DisposeCurrentContext();
    }
}
