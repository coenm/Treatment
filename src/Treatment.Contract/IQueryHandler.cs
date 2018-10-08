namespace Treatment.Contract
{
    using System;
    using System.Threading;

    using JetBrains.Annotations;

    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        TResult Handle(TQuery query, [CanBeNull] IProgress<ProgressData> progress = null, CancellationToken ct = default(CancellationToken));
    }
}