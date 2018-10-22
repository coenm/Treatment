namespace Treatment.Plugin.Svn.Implementation
{
    using Treatment.Contract.Plugin.SourceControl;
    using Treatment.Core.Interfaces;

    internal class SvnSourceControlFactory : ISourceControlAbstractFactory
    {
        private readonly IFileSystem filesystem;

        public SvnSourceControlFactory(IFileSystem filesystem)
        {
            this.filesystem = filesystem;
        }

        public string Name { get; } = "Svn";

        public int Priority => 10;

        public IReadOnlySourceControl Create()
        {
            return new SvnReadOnlySourceControl(this.filesystem);
        }

        public bool CanCreate(string name)
        {
            return this.Name.Equals(name);
        }
    }
}
