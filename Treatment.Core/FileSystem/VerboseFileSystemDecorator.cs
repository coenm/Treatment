namespace Treatment.Core.FileSystem
{
    using System;

    using Treatment.Core.Interfaces;

    public class VerboseFileSystemDecorator : IFileSystem
    {
        private readonly IFileSystem _decoratee;
        private readonly IRootDirSanitizer _sanitizer;

        public VerboseFileSystemDecorator(IFileSystem decoratee, IRootDirSanitizer sanitizer)
        {
            _decoratee = decoratee;
            _sanitizer = sanitizer;
        }

        public string GetFileContent(string filename)
        {
            Console.WriteLine($"Get file content of '{_sanitizer.Sanitize(filename)}'");
            return _decoratee.GetFileContent(filename);
        }

        public void SaveContent(string filename, string content)
        {
            Console.WriteLine($"Save file '{_sanitizer.Sanitize(filename)}'");
            _decoratee.SaveContent(filename, content);
        }
    }
}