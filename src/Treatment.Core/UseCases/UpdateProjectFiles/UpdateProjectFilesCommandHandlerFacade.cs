namespace Treatment.Core.UseCases.UpdateProjectFiles
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Treatment.Contract;
    using Treatment.Contract.Commands;
    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.Core.Interfaces;

    /// <remarks>
    /// Try to separate logging or progress feedback from the core functionality.
    /// Not sure if this is the way to do this. Tried to use conditional registering decorators to simple injector.
    /// Now it acts like a lazy decorator.
    /// </remarks>>
    [UsedImplicitly]
    public class UpdateProjectFilesCommandHandlerFacade : ICommandHandler<UpdateProjectFilesCommand>
    {
        private readonly IFileSystem _filesystem;
        private readonly IFileSearch _fileSearcher;

        public UpdateProjectFilesCommandHandlerFacade([NotNull] IFileSystem filesystem, [NotNull] IFileSearch fileSearcher)
        {
            _filesystem = filesystem ?? throw new ArgumentNullException(nameof(filesystem));
            _fileSearcher = fileSearcher ?? throw new ArgumentNullException(nameof(fileSearcher));
        }

        public async Task ExecuteAsync(UpdateProjectFilesCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default(CancellationToken))
        {
            // so if progress is null, we don't have to decorate the filesystem and file searcher.
            // create the 'real' command handler and let it handle the command.
            if (progress == null)
            {
                var handler = new UpdateProjectFilesCommandHandlerImplementation(_filesystem, _fileSearcher);
                await handler.ExecuteAsync(command, null, ct).ConfigureAwait(false);
                return;
            }

            // progress is not null (progress feedback if desired).
            // Decorate the filesystem and fileSearcher to do the actual progress feedback (done in ProgressCommandExecution),
            // and instantiate the 'real' command handler and let it handle the command.
            using (var progressCommandExecution = new ProgressCommandExecution(_filesystem, _fileSearcher, progress))
            {
                var handler = new UpdateProjectFilesCommandHandlerImplementation(progressCommandExecution, progressCommandExecution);
                await handler.ExecuteAsync(command, null, ct).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// This class decorates <see cref="IFileSystem"/> and <see cref="IFileSearch"/>
        /// in order to report to <see cref="IProgress{ProgressData}"/>
        /// when <see cref="UpdateProjectFilesCommandHandlerImplementation"/> is handling a <see cref="UpdateProjectFilesCommand"/>
        /// </summary>
        private class ProgressCommandExecution : IFileSystem, IFileSearch, IDisposable
        {
            [NotNull]
            private readonly IFileSystem _filesystem;

            [NotNull]
            private readonly IFileSearch _fileSearch;

            [NotNull]
            private readonly IProgress<ProgressData> _progress;

            private int _foundFileCount;
            private int _currentIndex;

            internal ProgressCommandExecution([NotNull] IFileSystem filesystem, [NotNull] IFileSearch fileSearch, [NotNull] IProgress<ProgressData> progress)
            {
                _filesystem = filesystem ?? throw new ArgumentNullException(nameof(filesystem));
                _fileSearch = fileSearch ?? throw new ArgumentNullException(nameof(fileSearch));
                _progress = progress ?? throw new ArgumentNullException(nameof(progress));
                _foundFileCount = 0;
            }

            public void Dispose()
            {
                _progress.Report(new ProgressData("Done"));
            }

            bool IFileSystem.FileExists(string filename) => _filesystem.FileExists(filename);

            Stream IFileSystem.ReadFile(string filename) => _filesystem.ReadFile(filename);

            string IFileSystem.GetFileContent(string filename)
            {
                _currentIndex++;
                _progress.Report(new ProgressData(_currentIndex, _foundFileCount, $"Processing '{filename}'"));
                return _filesystem.GetFileContent(filename);
            }

            void IFileSystem.SaveContent(string filename, string content)
            {
                _progress.Report(new ProgressData(_currentIndex, _foundFileCount, $"File {filename} updated"));
                _filesystem.SaveContent(filename, content);
            }

            Task IFileSystem.SaveContentAsync(string filename, Stream content) => _filesystem.SaveContentAsync(filename, content);

            void IFileSystem.DeleteFile(string filename) => _filesystem.DeleteFile(filename);

            string[] IFileSearch.FindFilesIncludingSubdirectories(string rootPath, string mask)
            {
                _progress.Report(new ProgressData($"Find cs files within directory {rootPath}"));

                var result = _fileSearch.FindFilesIncludingSubdirectories(rootPath, mask);

                _foundFileCount = result.Length;
                _currentIndex = 0;

                if (_foundFileCount == 0)
                    _progress.Report(new ProgressData("No files found"));

                return result;
            }
        }
    }
}