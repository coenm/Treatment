namespace Treatment.Console.Decorators
{
    using JetBrains.Annotations;

    using Treatment.Console.Console;
    using Treatment.Core.Interfaces;

    [UsedImplicitly]
    public class VerboseFileSystemDecorator : IFileSystem
    {
        private readonly IFileSystem _decoratee;
        private readonly IRootDirSanitizer _sanitizer;
        private readonly IConsole _console;

        public VerboseFileSystemDecorator(IFileSystem decoratee, IRootDirSanitizer sanitizer, IConsole console)
        {
            _decoratee = decoratee;
            _sanitizer = sanitizer;
            _console = console;
        }

        public string GetFileContent(string filename)
        {
            _console.WriteLine($"Get file content of '{_sanitizer.Sanitize(filename)}'");
            return _decoratee.GetFileContent(filename);
        }

        public void SaveContent(string filename, string content)
        {
            _console.WriteLine($"Save file '{_sanitizer.Sanitize(filename)}'");
            _decoratee.SaveContent(filename, content);
        }
    }
}