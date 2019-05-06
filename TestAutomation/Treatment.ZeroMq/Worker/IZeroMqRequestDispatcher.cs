namespace TreatmentZeroMq.Worker
{
    using System.Threading.Tasks;
    using ZeroMQ;

    public interface IZeroMqRequestDispatcher
    {
        Task<ZMessage> ProcessAsync(ZMessage message);
    }
}
