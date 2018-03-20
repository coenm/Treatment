namespace Treatment.Core.FileSystem
{
    using System;

    using JetBrains.Annotations;

    using Treatment.Core.Interfaces;

    [UsedImplicitly]
    public class DryRunFileSystemDecorator : IFileSystem
    {
        private readonly IFileSystem _decoratee;
        private readonly IRootDirSanitizer _sanitizer;

        public DryRunFileSystemDecorator(IFileSystem decoratee, IRootDirSanitizer sanitizer)
        {
            _decoratee = decoratee;
            _sanitizer = sanitizer;
        }

        public string GetFileContent(string filename)
        {
            return _decoratee.GetFileContent(filename);
        }

        public void SaveContent(string filename, string content)
        {
            Console.WriteLine($"Would save content to '{_sanitizer.Sanitize(filename)}'");
        }
    }
}