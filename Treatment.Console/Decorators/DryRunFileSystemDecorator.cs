namespace Treatment.Console.Decorators
{
    using JetBrains.Annotations;

    using Treatment.Console.Console;
    using Treatment.Core.Interfaces;

    /// <summary>
    /// Decorate IFileSystem not to save data to disk
    /// </summary>
    [UsedImplicitly]
    public class DryRunFileSystemDecorator : IFileSystem
    {
        private readonly IFileSystem _decoratee;
        private readonly IRootDirSanitizer _sanitizer;
        private readonly IConsole _console;

        public DryRunFileSystemDecorator(IFileSystem decoratee, IRootDirSanitizer sanitizer, IConsole console)
        {
            _decoratee = decoratee;
            _sanitizer = sanitizer;
            _console = console;
        }

        public string GetFileContent(string filename)
        {
            return _decoratee.GetFileContent(filename);
        }

        public void SaveContent(string filename, string content)
        {
            _console.WriteLine($"Would save content to '{_sanitizer.Sanitize(filename)}'");
        }
    }
}