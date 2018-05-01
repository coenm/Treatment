namespace Treatment.Core.DefaultPluginImplementation.SourceControl
{
    using JetBrains.Annotations;

    using Treatment.Contract.Plugin.SourceControl;

    [UsedImplicitly]
    internal class DummySourceControlFactory : ISourceControlAbstractFactory
    {
        public string Name { get; } = "dummy";

        public int Priority { get; } = int.MaxValue;

        public IReadOnlySourceControl Create()
        {
            return DummyReadOnlySourceControl.Instance;
        }

        public bool CanCreate(string name) => true;
    }
}