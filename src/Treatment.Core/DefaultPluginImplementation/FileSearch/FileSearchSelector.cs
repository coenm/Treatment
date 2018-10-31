namespace Treatment.Core.DefaultPluginImplementation.FileSearch
{
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.Helpers;

    [UsedImplicitly]
    internal class FileSearchSelector : IFileSearchSelector
    {
        [NotNull] private readonly IEnumerable<ISearchProviderFactory> factories;
        [NotNull] private readonly ISearchProviderNameOption searchProviderName;

        public FileSearchSelector(
            [NotNull] IEnumerable<ISearchProviderFactory> factories,
            [NotNull] ISearchProviderNameOption searchProviderName)
        {
            this.factories = Guard.NotNull(factories, nameof(factories));
            this.searchProviderName = Guard.NotNull(searchProviderName, nameof(searchProviderName));
        }

        [CanBeNull]
        public IFileSearch CreateSearchProvider()
        {
            var factory = factories
                          .OrderBy(f => f.Priority)
                          .FirstOrDefault(item => item.CanCreate(searchProviderName.SearchProviderName));

            return factory?.Create();
        }
    }
}
