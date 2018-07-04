namespace Treatment.Core.DefaultPluginImplementation.SourceControl
{
    using Treatment.Contract.Plugin.SourceControl;

    public interface ISourceControlSelector
    {
        IReadOnlySourceControl CreateSourceControl();
    }
}