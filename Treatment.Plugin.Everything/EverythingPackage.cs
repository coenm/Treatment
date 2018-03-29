namespace Treatment.Plugin.Everything
{
    using JetBrains.Annotations;

    using SimpleInjector;
    using SimpleInjector.Advanced;
    using SimpleInjector.Packaging;

    using Treatment.Core.Interfaces;

    [UsedImplicitly]
    public class EverythingPackage : IPackage
    {
        public void RegisterServices(Container container)
        {
            if (Everything32Api.IsEverythingAvailable())
                container.AppendToCollection(typeof(ISearchProviderFactory), typeof(EverythingFileSeachFactory));
        }
    }
}