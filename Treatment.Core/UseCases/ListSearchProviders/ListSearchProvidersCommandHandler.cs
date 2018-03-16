namespace Treatment.Core.UseCases.ListSearchProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using Treatment.Core.Interfaces;
    using Treatment.Core.UseCases;

    [UsedImplicitly]
    public class ListSearchProvidersCommandHandler : ICommandHandler<ListSearchProvidersCommand>
    {
        private readonly IEnumerable<ISearchProviderFactory> _searchProviderFactories;

        public ListSearchProvidersCommandHandler(IEnumerable<ISearchProviderFactory> searchProviderFactories)
        {
            _searchProviderFactories = searchProviderFactories.ToList();
        }

        public void Execute(ListSearchProvidersCommand command)
        {
            var orderedFactories = _searchProviderFactories.OrderBy(f => f.Priority);

            Console.WriteLine("Installed search providers (ordered by priority):");
            foreach (var f in orderedFactories)
                Console.WriteLine($"- {f.Name}");
        }
    }
}