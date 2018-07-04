namespace Treatment.Console.Decorators
{
    using System;
    using System.Diagnostics;

    using JetBrains.Annotations;

    using Treatment.Console.Console;
    using Treatment.Contract.Plugin.FileSearch;

    [UsedImplicitly]
    public class VerboseFileSearchDecorator : IFileSearch
    {
        private readonly IFileSearch _decoratee;
        private readonly IConsole _console;

        public VerboseFileSearchDecorator(IFileSearch decoratee, IConsole console)
        {
            _decoratee = decoratee;
            _console = console;
        }

        public string[] FindFilesIncludingSubdirectories(string rootPath, string mask)
        {
            _console.WriteLine("Find files");

            var result = new string[0];
            Exception ex = null;

            var sw = Stopwatch.StartNew();
            try
            {
                result = _decoratee.FindFilesIncludingSubdirectories(rootPath, mask);
            }
            catch (Exception e)
            {
                ex = e;
                throw;
            }
            finally
            {
                sw.Stop();
                _console.WriteLine(ex == null
                                       ? $"Found {result.Length} csproj files to process in {sw.Elapsed}"
                                       : $"An exception occurred during the search of csproj files in {sw.Elapsed}");
            }

            return result;
        }
    }
}