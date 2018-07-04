namespace Treatment.Plugin.Svn
{
    using JetBrains.Annotations;

    using SimpleInjector;
    using SimpleInjector.Packaging;

    using Treatment.Contract.Plugin.SourceControl;
    using Treatment.Plugin.Svn.Implementation;

    [UsedImplicitly]
    public class SvnPackage : IPackage
    {
        public void RegisterServices(Container container)
        {
            container?.Collection.Append(typeof(ISourceControlAbstractFactory), typeof(SvnSourceControlFactory));
        }
    }
}
