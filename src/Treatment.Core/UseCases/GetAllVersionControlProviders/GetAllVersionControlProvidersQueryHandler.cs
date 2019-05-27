namespace Treatment.Core.UseCases.GetAllVersionControlProviders
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Treatment.Contract;
    using Treatment.Contract.DTOs;
    using Treatment.Contract.Plugin.SourceControl;
    using Treatment.Contract.Queries;
    using Treatment.Helpers.Guards;

    [UsedImplicitly]
    public class GetAllVersionControlProvidersQueryHandler : IQueryHandler<GetAllVersionControlProvidersQuery, List<VersionControlProviderInfo>>
    {
        private readonly List<ISourceControlAbstractFactory> searchProviderFactories;

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "Nullcheck")]
        public GetAllVersionControlProvidersQueryHandler([NotNull] IEnumerable<ISourceControlAbstractFactory> versionControlProviderFactories)
        {
            Guard.NotNull(versionControlProviderFactories, nameof(versionControlProviderFactories));
            searchProviderFactories = versionControlProviderFactories.ToList();
        }

        public Task<List<VersionControlProviderInfo>> HandleAsync(GetAllVersionControlProvidersQuery query, IProgress<ProgressData> progress = null, CancellationToken ct = default(CancellationToken))
        {
            return Task.FromResult(searchProviderFactories
                                   .OrderBy(f => f.Priority)
                                   .Select(f => new VersionControlProviderInfo(f.Priority, f.Name))
                                   .ToList());
        }
    }
}
