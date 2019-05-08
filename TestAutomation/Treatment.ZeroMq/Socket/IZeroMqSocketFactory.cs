namespace TreatmentZeroMq.Socket
{
    using ZeroMQ;

    public interface IZeroMqSocketFactory
    {
        ZSocket Create(ZSocketType socketType);
    }
}
