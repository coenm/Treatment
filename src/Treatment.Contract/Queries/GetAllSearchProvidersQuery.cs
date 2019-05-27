namespace Treatment.Contract.Queries
{
    using System.Collections.Generic;

    using Treatment.Contract;
    using Treatment.Contract.DTOs;

    public class GetAllSearchProvidersQuery : IQuery<List<SearchProviderInfo>>
    {
        private GetAllSearchProvidersQuery()
        {
        }

        public static GetAllSearchProvidersQuery Instance { get; } = new GetAllSearchProvidersQuery();
    }
}
