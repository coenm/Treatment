namespace Treatment.Plugin.Svn.Implementation
{
    using Treatment.Contract.Plugin.SourceControl;
    using Treatment.Core.Interfaces;

    internal class SvnSourceControlFactory : ISourceControlAbstractFactory
    {
        private readonly IFileSystem _filesystem;

        public SvnSourceControlFactory(IFileSystem filesystem)
        {
            _filesystem = filesystem;
        }

        public string Name { get; } = "Svn";

        public int Priority => 10;

        public IReadOnlySourceControl Create()
        {
            return new SvnReadOnlySourceControl(_filesystem);
        }

        public bool CanCreate(string name)
        {
            return Name.Equals(name);
        }
    }
}