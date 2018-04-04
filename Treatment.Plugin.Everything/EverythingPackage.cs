namespace Treatment.Plugin.Everything
{
    using JetBrains.Annotations;

    using SimpleInjector;
    using SimpleInjector.Packaging;

    using Treatment.Core.Interfaces;

    [UsedImplicitly]
    public class EverythingPackage : IPackage
    {
        public void RegisterServices(Container container)
        {
            if (container == null)
                return;

            if (Everything32Api.IsInstalled())
                container.Collections.AppendTo(typeof(ISearchProviderFactory), typeof(EverythingFileSeachFactory));
        }
    }
}