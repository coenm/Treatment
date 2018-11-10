namespace Treatment.Core.Bootstrap
{
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using SimpleInjector;
    using Treatment.Contract;
    using Treatment.Helpers.Guards;

    [UsedImplicitly]
    public class QueryProcessor : IQueryProcessor
    {
        [NotNull] private readonly Container container;

        public QueryProcessor([NotNull] Container container)
        {
            Guard.NotNull(container, nameof(container));
            this.container = container;
        }

        public async Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query, CancellationToken ct = default(CancellationToken))
        {
            Guard.NotNull(query, nameof(query));

            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = container.GetInstance(handlerType);

            return await handler.HandleAsync((dynamic)query, null, ct);
        }
    }
}
