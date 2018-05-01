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
        [NotNull] private readonly IFileSystem _decoratee;
        [NotNull] private readonly IRootDirSanitizer _sanitizer;
        [NotNull] private readonly IConsole _console;

        public DryRunFileSystemDecorator(
            [NotNull] IFileSystem decoratee,
            [NotNull] IRootDirSanitizer sanitizer,
            [NotNull] IConsole console)
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