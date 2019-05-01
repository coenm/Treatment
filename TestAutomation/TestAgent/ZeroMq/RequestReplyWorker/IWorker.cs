namespace TestAgent.ZeroMq.RequestReplyWorker
{
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using TestAgent.Implementation;

    public interface IWorker
    {
        Task StartSingleWorker(
            [NotNull] IZeroMqRequestDispatcher messageDispatcher,
            [NotNull] string backendAddress,
            CancellationToken ct = default);
    }
}