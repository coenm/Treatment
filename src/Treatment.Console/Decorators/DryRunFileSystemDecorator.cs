namespace Treatment.Console.Decorators
{
    using System.IO;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Treatment.Console.Console;
    using Treatment.Core.Interfaces;

    /// <summary>
    /// Decorate IFileSystem not to save data to disk
    /// </summary>
    [UsedImplicitly]
    public class DryRunFileSystemDecorator : IFileSystem
    {
        [NotNull]
        private readonly IFileSystem _decoratee;

        [NotNull]
        private readonly IRootDirSanitizer _sanitizer;

        [NotNull]
        private readonly IConsole _console;

        public DryRunFileSystemDecorator(
            [NotNull] IFileSystem decoratee,
            [NotNull] IRootDirSanitizer sanitizer,
            [NotNull] IConsole console)
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
            return _decoratee.ReadFile(filename);
        }

        public string GetFileContent(string filename)
        {
            return _decoratee.GetFileContent(filename);
        }

        public void SaveContent(string filename, string content)
        {
            DummySaveContent(filename);
        }

        public Task SaveContentAsync(string filename, Stream content)
        {
            DummySaveContent(filename);
            return Task.CompletedTask;
        }

        public void DeleteFile(string filename)
        {
            _console.WriteLine($"Would have deleted '{_sanitizer.Sanitize(filename)}'");
        }

        private void DummySaveContent(string filename)
        {
            _console.WriteLine($"Would save content to '{_sanitizer.Sanitize(filename)}'");
        }
    }
}