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
        private readonly IFileSearch decoratee;
        private readonly IConsole console;

        public VerboseFileSearchDecorator(IFileSearch decoratee, IConsole console)
        {
            this.decoratee = decoratee;
            this.console = console;
        }

        public string[] FindFilesIncludingSubdirectories(string rootPath, string mask)
        {
            console.WriteLine("Find files");

            var result = new string[0];
            Exception ex = null;

            var sw = Stopwatch.StartNew();
            try
            {
                result = decoratee.FindFilesIncludingSubdirectories(rootPath, mask);
            }
            catch (Exception e)
            {
                ex = e;
                throw;
            }
            finally
            {
                sw.Stop();
                console.WriteLine(ex == null
                                       ? $"Found {result.Length} csproj files to process in {sw.Elapsed}"
                                       : $"An exception occurred during the search of csproj files in {sw.Elapsed}");
            }

            return result;
        }
    }
}
