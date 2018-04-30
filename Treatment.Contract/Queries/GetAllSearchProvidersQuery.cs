namespace Treatment.Contract.Queries
{
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Treatment.Contract;
    using Treatment.Contract.DTOs;

    [UsedImplicitly]
    public class GetAllSearchProvidersQuery : IQuery<List<SearchProviderInfo>>
    {
    }
}