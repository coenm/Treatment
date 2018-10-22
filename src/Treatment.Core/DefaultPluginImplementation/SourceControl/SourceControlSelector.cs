namespace Treatment.Core.DefaultPluginImplementation.SourceControl
{
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using Treatment.Contract.Plugin.SourceControl;

    [UsedImplicitly]
    internal class SourceControlSelector : ISourceControlSelector
    {
        [NotNull]
        private readonly IEnumerable<ISourceControlAbstractFactory> factories;

        [NotNull]
        private readonly ISourceControlNameOption searchProviderName;

        public SourceControlSelector(
            [NotNull] IEnumerable<ISourceControlAbstractFactory> factories,
            [NotNull] ISourceControlNameOption searchProviderName)
        {
            this.factories = factories;
            this.searchProviderName = searchProviderName;
        }

        [NotNull]
        public IReadOnlySourceControl CreateSourceControl()
        {
            var factory = factories
                          .OrderBy(f => f.Priority)
                          .FirstOrDefault(item => item.CanCreate(searchProviderName.SourceControlProviderName));

            if (factory == null)
                return new DummySourceControlFactory().Create();

            return factory.Create();
        }
    }
}
