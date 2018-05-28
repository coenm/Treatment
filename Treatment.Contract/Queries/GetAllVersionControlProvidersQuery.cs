namespace Treatment.Contract.Queries
{
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Treatment.Contract.DTOs;

    [UsedImplicitly]
    public class GetAllVersionControlProvidersQuery : IQuery<List<VersionControlProviderInfo>>
    {
    }
}