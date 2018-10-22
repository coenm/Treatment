﻿namespace Treatment.Console.Decorators
{
    using System.IO;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Treatment.Console.Console;
    using Treatment.Core.Interfaces;

    [UsedImplicitly]
    public class VerboseFileSystemDecorator : IFileSystem
    {
        private readonly IFileSystem decoratee;
        private readonly IRootDirSanitizer sanitizer;
        private readonly IConsole console;

        public VerboseFileSystemDecorator(IFileSystem decoratee, IRootDirSanitizer sanitizer, IConsole console)
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
            console.WriteLine($"Read file '{sanitizer.Sanitize(filename)}'");
            return decoratee.ReadFile(filename);
        }

        public string GetFileContent(string filename)
        {
            console.WriteLine($"Get file content of '{sanitizer.Sanitize(filename)}'");
            return decoratee.GetFileContent(filename);
        }

        public void SaveContent(string filename, string content)
        {
            console.WriteLine($"Save file '{sanitizer.Sanitize(filename)}'");
            decoratee.SaveContent(filename, content);
        }

        public async Task SaveContentAsync(string filename, Stream content)
        {
            console.WriteLine($"Save file '{sanitizer.Sanitize(filename)}'");
            await decoratee.SaveContentAsync(filename, content).ConfigureAwait(false);
        }

        public void DeleteFile(string filename)
        {
            console.WriteLine($"Delete file '{sanitizer.Sanitize(filename)}'");
            decoratee.DeleteFile(filename);
        }
    }
}
