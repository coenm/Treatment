namespace Treatment.Core.FileSearch
{
    using System;
    using System.Diagnostics;

    using JetBrains.Annotations;

    using Treatment.Core.Interfaces;

    [UsedImplicitly]
    public class VerboseFileSearchDecorator : IFileSearch
    {
        private readonly IFileSearch _decoratee;

        public VerboseFileSearchDecorator(IFileSearch decoratee)
        {
            _decoratee = decoratee;
        }

        public string[] FindFilesIncludingSubdirectories(string rootPath, string mask)
        {
            Console.WriteLine("Find files");
            var sw = Stopwatch.StartNew();
            var result = _decoratee.FindFilesIncludingSubdirectories(rootPath, mask);
            sw.Stop();
            Console.WriteLine($"Found {result.Length} csproj files to process in {sw.Elapsed}");
            return result;
        }
    }
}