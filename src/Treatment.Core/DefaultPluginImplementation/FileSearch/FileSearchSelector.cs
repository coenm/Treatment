namespace Treatment.Core.DefaultPluginImplementation.FileSearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using Treatment.Contract.Plugin.FileSearch;

    [UsedImplicitly]
    internal class FileSearchSelector : IFileSearchSelector
    {
        [NotNull]
        private readonly IEnumerable<ISearchProviderFactory> _factories;
        [NotNull]
        private readonly ISearchProviderNameOption _searchProviderName;

        public FileSearchSelector(
            [NotNull] IEnumerable<ISearchProviderFactory> factories,
            [NotNull] ISearchProviderNameOption searchProviderName)
        {
            _factories = factories ?? throw new ArgumentNullException(nameof(factories));
            _searchProviderName = searchProviderName ?? throw new ArgumentNullException(nameof(searchProviderName));
        }

        [CanBeNull]
        public IFileSearch CreateSearchProvider()
        {
            var factory = _factories
                          .OrderBy(f => f.Priority)
                          .FirstOrDefault(item => item.CanCreate(_searchProviderName.SearchProviderName));

            return factory?.Create();
        }
    }
}