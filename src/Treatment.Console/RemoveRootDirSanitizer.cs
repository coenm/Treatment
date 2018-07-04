namespace Treatment.Console
{
    public class RemoveRootDirSanitizer : IRootDirSanitizer
    {
        private string _rootDir;

        public RemoveRootDirSanitizer()
        {
            _rootDir = string.Empty;
        }

        public void SetRootDir(string input)
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