namespace Treatment.Console.Decorators
{
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Treatment.Console.Console;
    using Treatment.Core.Interfaces;

    [UsedImplicitly]
    public class StatisticsCollectorAndSummaryWriter : IStatistics, ISummaryWriter
    {
        private readonly IConsole _console;
        private readonly List<string> _filesRead = new List<string>();
        private readonly List<string> _filesChanged = new List<string>();
        private readonly List<string> _foundFiles = new List<string>();

        public StatisticsCollectorAndSummaryWriter(IConsole console)
        {
            _console = console;
        }

        public void AddFileRead(string filename)
        {
            _filesRead.Add(filename);
        }

        public void AddFileUpdate(string filename)
        {
            _filesChanged.Add(filename);
        }

        public void AddFoundFiles(string[] filenames)
        {
            _foundFiles.AddRange(filenames);
        }

        public void OutputSummary()
        {
            _console.WriteLine(string.Empty);
            _console.WriteLine("Summary:");
            _console.WriteLine($"- Files found: {_foundFiles.Count}");
            _console.WriteLine($"- Files read: {_filesRead.Count}");
            _console.WriteLine($"- Files updated: {_filesChanged.Count}");
        }
    }
}