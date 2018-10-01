namespace Treatment.Console.Decorators
{
    using System.IO;
    using System.Threading.Tasks;

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

        public bool FileExists(string filename)
        {
            return _decoratee.FileExists(filename);
        }

        public Stream ReadFile(string filename)
        {
            _console.WriteLine($"Read file '{_sanitizer.Sanitize(filename)}'");
            return _decoratee.ReadFile(filename);
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

        public async Task SaveContentAsync(string filename, Stream content)
        {
            _console.WriteLine($"Save file '{_sanitizer.Sanitize(filename)}'");
            await _decoratee.SaveContentAsync(filename, content).ConfigureAwait(false);
        }

        public void DeleteFile(string filename)
        {
            _console.WriteLine($"Delete file '{_sanitizer.Sanitize(filename)}'");
            _decoratee.DeleteFile(filename);
        }
    }
}