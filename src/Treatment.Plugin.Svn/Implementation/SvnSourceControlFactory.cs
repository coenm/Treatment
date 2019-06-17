namespace Treatment.Plugin.Svn.Implementation
{
    using Treatment.Helpers.FileSystem;
    using JetBrains.Annotations;
    using Treatment.Contract.Plugin.SourceControl;
    using Treatment.Core.Interfaces;
    using Treatment.Helpers.Guards;

    internal class SvnSourceControlFactory : ISourceControlAbstractFactory
    {
        private readonly IFileSystem filesystem;

        public SvnSourceControlFactory([NotNull] IFileSystem filesystem)
        {
            Guard.NotNull(filesystem, nameof(filesystem));
            this.filesystem = filesystem;
        }

        public string Name { get; } = "Svn";

        public int Priority => 10;

        public IReadOnlySourceControl Create()
        {
            return new SvnReadOnlySourceControl(filesystem);
        }

        public bool CanCreate(string name)
        {
            return Name.Equals(name);
        }
    }
}
