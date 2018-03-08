namespace Treatment.Core.Statistics
{
    using System;
    using System.Collections.Generic;

    using Treatment.Core.Interfaces;

    public class StatisticsCollectorAndSummaryWriter : IStatistics, ISummaryWriter
    {
        private readonly List<string> _filesRead = new List<string>();
        private readonly List<string> _filesChanged = new List<string>();
        private readonly List<string> _foundFiles = new List<string>();

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
            Console.WriteLine(string.Empty);
            Console.WriteLine("Summary:");
            Console.WriteLine($"- Files found: {_foundFiles.Count}");
            Console.WriteLine($"- Files read: {_filesRead.Count}");
            Console.WriteLine($"- Files updated: {_filesChanged.Count}");
        }
    }
}