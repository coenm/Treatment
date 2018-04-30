namespace Treatment.Core
{
    using JetBrains.Annotations;

    using Treatment.Core.Interfaces;

    /*internal*/public class RemoveRootDirSanitizer : IRootDirSanitizer
    {
        private string _rootDir;

        public RemoveRootDirSanitizer()
        {
            _rootDir = string.Empty;
        }

        public void SetRootDir([NotNull] string input)
        {
            _rootDir = input;
        }

        public string Sanitize(string input)
        {
            // needs some work.
            return input.Replace(_rootDir, string.Empty);
        }
    }
}