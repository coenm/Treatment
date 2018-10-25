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
        private readonly IEnumerable<ISearchProviderFactory> factories;
        [NotNull]
        private readonly ISearchProviderNameOption searchProviderName;

        public FileSearchSelector(
            [NotNull] IEnumerable<ISearchProviderFactory> factories,
            [NotNull] ISearchProviderNameOption searchProviderName)
        {
            this.factories = factories ?? throw new ArgumentNullException(nameof(factories));
            this.searchProviderName = searchProviderName ?? throw new ArgumentNullException(nameof(searchProviderName));
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
