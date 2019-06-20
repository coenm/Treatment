namespace CoenM.ZeroMq.Socket
{
    using ZeroMQ;

    public interface IZeroMqSocketFactory
    {
        ZSocket Create(ZSocketType socketType);
    }
}
