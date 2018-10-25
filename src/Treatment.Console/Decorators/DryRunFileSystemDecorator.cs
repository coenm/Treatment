namespace Treatment.Console.Decorators
{
    using System.IO;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Treatment.Console.Console;
    using Treatment.Core.Interfaces;

    /// <summary>
    /// Decorate IFileSystem not to save data to disk.
    /// </summary>
    [UsedImplicitly]
    public class DryRunFileSystemDecorator : IFileSystem
    {
        [NotNull]
        private readonly IFileSystem decoratee;

        [NotNull]
        private readonly IRootDirSanitizer sanitizer;

        [NotNull]
        private readonly IConsole console;

        public DryRunFileSystemDecorator(
            [NotNull] IFileSystem decoratee,
            [NotNull] IRootDirSanitizer sanitizer,
            [NotNull] IConsole console)
        {
            this.decoratee = decoratee;
            this.sanitizer = sanitizer;
            this.console = console;
        }

        public bool FileExists(string filename)
        {
            return decoratee.FileExists(filename);
        }

        public Stream ReadFile(string filename)
        {
            return decoratee.ReadFile(filename);
        }

        public string GetFileContent(string filename)
        {
            return decoratee.GetFileContent(filename);
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
            console.WriteLine($"Would have deleted '{sanitizer.Sanitize(filename)}'");
        }

        private void DummySaveContent(string filename)
        {
            console.WriteLine($"Would save content to '{sanitizer.Sanitize(filename)}'");
        }
    }
}
