namespace Treatment.Contract
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    public interface IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        [UsedImplicitly] // using reflection.
        Task<TResult> HandleAsync(TQuery query, [CanBeNull] IProgress<ProgressData> progress = null, CancellationToken ct = default);
    }
}
