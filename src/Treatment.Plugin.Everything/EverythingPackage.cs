namespace Treatment.Plugin.Everything
{
    using JetBrains.Annotations;

    using SimpleInjector;
    using SimpleInjector.Packaging;

    using Treatment.Contract.Plugin.FileSearch;

    [UsedImplicitly]
    public class EverythingPackage : IPackage
    {
        public void RegisterServices([NotNull] Container container)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (container == null)
                return;

            if (Everything64Api.IsInstalled())
                container.Collection.Append(typeof(ISearchProviderFactory), typeof(EverythingFileSearchFactory));
        }
    }
}