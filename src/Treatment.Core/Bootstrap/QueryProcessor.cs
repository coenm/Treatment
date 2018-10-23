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
        [NotNull]
        private readonly Container container;

        public QueryProcessor([NotNull] Container container)
        {
            this.container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public async Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query, CancellationToken ct = default)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = container.GetInstance(handlerType);

            return await handler.HandleAsync((dynamic)query, null, ct);
        }
    }
}
