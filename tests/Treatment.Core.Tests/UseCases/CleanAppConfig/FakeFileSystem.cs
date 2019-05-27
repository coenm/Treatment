namespace Treatment.Core.Tests.UseCases.CleanAppConfig
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Treatment.Contract.Plugin.SourceControl;

    internal class FakeFileSystem
    {
        private readonly List<FakeFileItem> fileSystem;

        public FakeFileSystem()
        {
            fileSystem = new List<FakeFileItem>();
        }

        public void Add(string filename, FileStatus state)
        {
            fileSystem.Add(new FakeFileItem(filename, state));
        }

        public void Clear()
        {
            fileSystem.Clear();
        }

        public string[] GetFiles()
        {
            return fileSystem.Select(x => x.Filename).ToArray();
        }

        public FileStatus GetFileState(string filename)
        {
            var result = fileSystem.FirstOrDefault(x => x.Filename == filename);

            if (result == null)
                throw new Exception();

            return result.State;
        }

        private class FakeFileItem
        {
            public FakeFileItem(string filename, FileStatus state)
            {
                Filename = filename;
                State = state;
            }

            public string Filename { get; }

            public FileStatus State { get; }
        }
    }
}
