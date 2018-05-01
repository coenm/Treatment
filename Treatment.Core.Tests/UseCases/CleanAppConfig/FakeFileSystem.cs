namespace Treatment.Core.Tests.UseCases.CleanAppConfig
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Treatment.Contract.Plugin.SourceControl;

    internal class FakeFileSystem
    {
        private readonly List<FakeFileItem> _fileSystem;

        public FakeFileSystem()
        {
            _fileSystem = new List<FakeFileItem>();
        }

        public void Add(string filename, FileStatus state)
        {
            _fileSystem.Add(new FakeFileItem(filename, state));
        }

        public void Clear()
        {
            _fileSystem.Clear();
        }

        public string[] GetFiles()
        {
            return _fileSystem.Select(x => x.Filename).ToArray();
        }

        public FileStatus GetFileState(string filename)
        {
            var result = _fileSystem.FirstOrDefault(x => x.Filename == filename);

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