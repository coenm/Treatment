namespace Treatment.Core.DefaultPluginImplementation.SourceControl
{
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using Treatment.Contract.Plugin.SourceControl;

    [UsedImplicitly]
    internal class SourceControlSelector : ISourceControlSelector
    {
        [NotNull] private readonly IEnumerable<ISourceControlAbstractFactory> _factories;
        [NotNull] private readonly ISourceControlNameOption _searchProviderName;

        public SourceControlSelector(
            [NotNull] IEnumerable<ISourceControlAbstractFactory> factories,
            [NotNull] ISourceControlNameOption searchProviderName)
        {
            _factories = factories;
            _searchProviderName = searchProviderName;
        }

        [NotNull]
        public IReadOnlySourceControl CreateSourceControl()
        {
            var factory = _factories
                          .OrderBy(f => f.Priority)
                          .FirstOrDefault(item => item.CanCreate(_searchProviderName.SearchProviderName));

            if (factory == null)
                return new DummySourceControlFactory().Create();

            return factory.Create();
        }
    }
}