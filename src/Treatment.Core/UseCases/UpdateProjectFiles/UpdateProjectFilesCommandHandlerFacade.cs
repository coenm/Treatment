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
    using Treatment.Helpers.FileSystem;
    using Treatment.Helpers.Guards;

    /// <remarks>
    /// Try to separate logging or progress feedback from the core functionality.
    /// Not sure if this is the way to do this. Tried to use conditional registering decorators to simple injector.
    /// Now it acts like a lazy decorator.
    /// </remarks>>
    [UsedImplicitly]
    public class UpdateProjectFilesCommandHandlerFacade : ICommandHandler<UpdateProjectFilesCommand>
    {
        [NotNull] private readonly IFileSystem filesystem;
        [NotNull] private readonly IFileSearch fileSearcher;

        public UpdateProjectFilesCommandHandlerFacade([NotNull] IFileSystem filesystem, [NotNull] IFileSearch fileSearcher)
        {
            Guard.NotNull(filesystem, nameof(filesystem));
            Guard.NotNull(fileSearcher, nameof(fileSearcher));

            this.filesystem = filesystem;
            this.fileSearcher = fileSearcher;
        }

        public async Task ExecuteAsync(UpdateProjectFilesCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default)
        {
            // so if progress is null, we don't have to decorate the filesystem and file searcher.
            // create the 'real' command handler and let it handle the command.
            if (progress == null)
            {
                var handler = new UpdateProjectFilesCommandHandlerImplementation(filesystem, fileSearcher);
                await handler.ExecuteAsync(command, null, ct).ConfigureAwait(false);
                return;
            }

            // progress is not null (progress feedback if desired).
            // Decorate the filesystem and fileSearcher to do the actual progress feedback (done in ProgressCommandExecution),
            // and instantiate the 'real' command handler and let it handle the command.
            using (var progressCommandExecution = new ProgressCommandExecution(filesystem, fileSearcher, progress))
            {
                var handler = new UpdateProjectFilesCommandHandlerImplementation(progressCommandExecution, progressCommandExecution);
                await handler.ExecuteAsync(command, null, ct).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// This class decorates <see cref="IFileSystem"/> and <see cref="IFileSearch"/>
        /// in order to report to <see cref="IProgress{ProgressData}"/>
        /// when <see cref="UpdateProjectFilesCommandHandlerImplementation"/> is handling a <see cref="UpdateProjectFilesCommand"/>.
        /// </summary>
        private class ProgressCommandExecution : IFileSystem, IFileSearch, IDisposable
        {
            [NotNull] private readonly IFileSystem filesystem;
            [NotNull] private readonly IFileSearch fileSearch;
            [NotNull] private readonly IProgress<ProgressData> progress;
            [NotNull] private ProgressDataPosition position;

            private int foundFileCount;
            private int currentIndex;

            internal ProgressCommandExecution(
                [NotNull] IFileSystem filesystem,
                [NotNull] IFileSearch fileSearch,
                [NotNull] IProgress<ProgressData> progress)
            {
                Guard.NotNull(filesystem, nameof(filesystem));
                Guard.NotNull(fileSearch, nameof(fileSearch));
                Guard.NotNull(progress, nameof(progress));

                this.filesystem = filesystem;
                this.fileSearch = fileSearch;
                this.progress = progress;
                foundFileCount = 0;
                position = new ProgressDataPosition(0, 0);
            }

            public void Dispose()
            {
                progress.Report(ProgressData.FinishedSuccessfully());
            }

            bool IFileSystem.FileExists(string filename) => filesystem.FileExists(filename);

            Stream IFileSystem.OpenRead(string filename, bool useAsync) => filesystem.OpenRead(filename, useAsync);

            Stream IFileSystem.OpenWrite(string filename, bool useAsync) => filesystem.OpenWrite(filename, useAsync);

            string IFileSystem.GetFileContent(string filename)
            {
                position = position.CreateIncrementalPosition();
                progress.Report(ProgressData.InProgress(position));
                return filesystem.GetFileContent(filename);
            }

            void IFileSystem.SaveContent(string filename, string content)
            {
                filesystem.SaveContent(filename, content);
            }

            Task IFileSystem.SaveContentAsync(string filename, Stream content) => filesystem.SaveContentAsync(filename, content);

            void IFileSystem.DeleteFile(string filename) => filesystem.DeleteFile(filename);

            string[] IFileSearch.FindFilesIncludingSubdirectories(string rootPath, string mask)
            {
                var result = fileSearch.FindFilesIncludingSubdirectories(rootPath, mask);

                foundFileCount = result.Length;
                currentIndex = 0;
                position = new ProgressDataPosition(currentIndex, foundFileCount);

                return result;
            }
        }
    }
}
