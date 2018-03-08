namespace Treatment.Core
{
    using JetBrains.Annotations;

    using Treatment.Core.Interfaces;

    public class RemoveRootDirSanitizer : IRootDirSanitizer
    {
        private readonly string _rootDir;

        public RemoveRootDirSanitizer([NotNull] string rootDir)
        {
            _rootDir = rootDir;
        }

        public string Sanitize(string input)
        {
            // needs some work.
            return input.Replace(_rootDir, string.Empty);
        }
    }
}