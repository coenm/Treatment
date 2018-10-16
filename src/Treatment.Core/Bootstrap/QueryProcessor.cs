namespace Treatment.Core.Bootstrap
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using SimpleInjector;

    using Treatment.Contract;

    [UsedImplicitly]
    public class QueryProcessor : IQueryProcessor
    {
        [NotNull] private readonly Container _container;

        public QueryProcessor([NotNull] Container container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public async Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query, CancellationToken ct = default(CancellationToken))
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = _container.GetInstance(handlerType);

            return await handler.HandleAsync((dynamic)query, null, ct);
        }
    }
}