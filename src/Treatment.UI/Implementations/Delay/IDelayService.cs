namespace Treatment.UI.Implementations.Delay
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IDelayService
    {
        Task DelayAsync(CancellationToken ct = default(CancellationToken));
    }
}
