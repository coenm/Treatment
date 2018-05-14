namespace Treatment.Core.DefaultPluginImplementation.SourceControl
{
    using Treatment.Contract.Plugin.SourceControl;

    internal class DummyReadOnlySourceControl : IReadOnlySourceControl
    {
        private DummyReadOnlySourceControl()
        {
        }

        public static DummyReadOnlySourceControl Instance { get; } = new DummyReadOnlySourceControl();

        public FileStatus GetFileStatus(string filename)
        {
            return FileStatus.Unknown;
        }

        public string GetOriginal(string filename)
        {
            return null;
        }

        public string GetModifications(string fileName)
        {
            return null;
        }
    }
}