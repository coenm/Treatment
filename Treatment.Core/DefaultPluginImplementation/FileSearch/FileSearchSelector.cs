namespace Treatment.Core.DefaultPluginImplementation.FileSearch
{
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using Treatment.Contract.Plugin.FileSearch;

    internal class FileSearchSelector : IFileSearchSelector
    {
        private readonly IEnumerable<ISearchProviderFactory> _factories;
        private string _searchProviderName;

        public FileSearchSelector(IEnumerable<ISearchProviderFactory> factories)
        {
            _factories = factories;
        }

        public void SetRequestedSearchProvider(string searchProviderName)
        {
            _searchProviderName = searchProviderName;
        }

        [CanBeNull]
        public  IFileSearch CreateSearchProvider()
        {
            var factory = _factories
                          .OrderBy(f => f.Priority)
                          .FirstOrDefault(item => item.CanCreate(_searchProviderName));

            return factory?.Create();
        }
    }

    public interface IFileSearchSelector
    {
        void SetRequestedSearchProvider(string searchProviderName);

        IFileSearch CreateSearchProvider();
    }
}