namespace Treatment.Core.UseCases.GetAllVersionControlProviders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using JetBrains.Annotations;

    using Treatment.Contract;
    using Treatment.Contract.DTOs;
    using Treatment.Contract.Plugin.SourceControl;
    using Treatment.Contract.Queries;

    [UsedImplicitly]
    public class GetAllVersionControlProvidersQueryHandler : IQueryHandler<GetAllVersionControlProvidersQuery, List<VersionControlProviderInfo>>
    {
        private readonly List<ISourceControlAbstractFactory> _searchProviderFactories;

        public GetAllVersionControlProvidersQueryHandler([NotNull] IEnumerable<ISourceControlAbstractFactory> versionControlProviderFactories)
        {
            _searchProviderFactories = versionControlProviderFactories.ToList();
        }

        public List<VersionControlProviderInfo> Handle(GetAllVersionControlProvidersQuery query, CancellationToken ct = default(CancellationToken))
        {
            return _searchProviderFactories
                   .OrderBy(f => f.Priority)
                   .Select(f => new VersionControlProviderInfo(f.Priority, f.Name))
                   .ToList();
        }
    }
}