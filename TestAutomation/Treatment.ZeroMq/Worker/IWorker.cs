namespace TreatmentZeroMq.Worker
{
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    public interface IWorker
    {
        Task StartSingleWorker(
            [NotNull] IZeroMqRequestDispatcher messageDispatcher,
            [NotNull] string backendAddress,
            CancellationToken ct = default);
    }
}