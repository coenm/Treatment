namespace Treatment.Console
{
    public class RemoveRootDirSanitizer : IRootDirSanitizer
    {
        private string rootDir;

        public RemoveRootDirSanitizer()
        {
            rootDir = string.Empty;
        }

        public void SetRootDir(string input)
        {
            rootDir = input;
        }

        public string Sanitize(string input)
        {
            // needs some work.
            return input.Replace(rootDir, string.Empty);
        }
    }
}
