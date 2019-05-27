namespace Treatment.Contract
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IQueryProcessor
    {
        Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query, CancellationToken ct = default(CancellationToken));
    }
}
