namespace Treatment.Core.UseCases.GetSearchProviders
{
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using Treatment.Contract;
    using Treatment.Contract.DTOs;
    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.Contract.Queries;
    using Treatment.Core.Interfaces;

    [UsedImplicitly]
    public class GetAllSearchProvidersQueryHandler : IQueryHandler<GetAllSearchProvidersQuery, List<SearchProviderInfo>>
    {
        private readonly IEnumerable<ISearchProviderFactory> _searchProviderFactories;

        public GetAllSearchProvidersQueryHandler([NotNull] IEnumerable<ISearchProviderFactory> searchProviderFactories)
        {
            _searchProviderFactories = searchProviderFactories.ToList();
        }

        public List<SearchProviderInfo> Handle(GetAllSearchProvidersQuery query)
        {
            var orderedFactories = _searchProviderFactories.OrderBy(f => f.Priority).ToList();

            var result = new List<SearchProviderInfo>(orderedFactories.Count);

            foreach (var f in orderedFactories)
                result.Add(new SearchProviderInfo(f.Priority, f.Name));

            return result;
        }
    }
}