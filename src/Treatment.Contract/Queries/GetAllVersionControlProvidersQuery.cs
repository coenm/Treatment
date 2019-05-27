namespace Treatment.Contract.Queries
{
    using System.Collections.Generic;

    using Treatment.Contract.DTOs;

    public class GetAllVersionControlProvidersQuery : IQuery<List<VersionControlProviderInfo>>
    {
        private GetAllVersionControlProvidersQuery()
        {
        }

        public static GetAllVersionControlProvidersQuery Instance { get; } = new GetAllVersionControlProvidersQuery();
    }
}