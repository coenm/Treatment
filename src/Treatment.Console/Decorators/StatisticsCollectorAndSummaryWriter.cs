namespace Treatment.Console.Decorators
{
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Treatment.Console.Console;
    using Treatment.Core.Interfaces;

    [UsedImplicitly]
    public class StatisticsCollectorAndSummaryWriter : IStatistics, ISummaryWriter
    {
        private readonly IConsole console;
        private readonly List<string> filesRead = new List<string>();
        private readonly List<string> filesChanged = new List<string>();
        private readonly List<string> foundFiles = new List<string>();

        public StatisticsCollectorAndSummaryWriter(IConsole console)
        {
            this.console = console;
        }

        public void AddFileRead(string filename)
        {
            filesRead.Add(filename);
        }

        public void AddFileUpdate(string filename)
        {
            filesChanged.Add(filename);
        }

        public void AddFoundFiles(string[] filenames)
        {
            foundFiles.AddRange(filenames);
        }

        public void OutputSummary()
        {
            console.WriteLine(string.Empty);
            console.WriteLine("Summary:");
            console.WriteLine($"- Files found: {foundFiles.Count}");
            console.WriteLine($"- Files read: {filesRead.Count}");
            console.WriteLine($"- Files updated: {filesChanged.Count}");
        }
    }
}
