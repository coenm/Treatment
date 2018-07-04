namespace Treatment.Plugin.Everything
{
    using JetBrains.Annotations;

    using SimpleInjector;
    using SimpleInjector.Packaging;

    using Treatment.Contract.Plugin.FileSearch;

    [UsedImplicitly]
    public class EverythingPackage : IPackage
    {
        public void RegisterServices(Container container)
        {
            if (container == null)
                return;

            if (Everything32Api.IsInstalled())
                container.Collection.Append(typeof(ISearchProviderFactory), typeof(EverythingFileSeachFactory));
        }
    }
}