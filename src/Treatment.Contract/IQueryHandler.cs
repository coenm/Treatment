namespace Treatment.Contract
{
    using System.Threading;

    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        TResult Handle(TQuery query, CancellationToken ct = default(CancellationToken));
    }
}