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
        private readonly IFileSystem filesystem;
        private readonly IFileSearch fileSearcher;

        public UpdateProjectFilesCommandHandlerFacade([NotNull] IFileSystem filesystem, [NotNull] IFileSearch fileSearcher)
        {
            this.filesystem = filesystem ?? throw new ArgumentNullException(nameof(filesystem));
            this.fileSearcher = fileSearcher ?? throw new ArgumentNullException(nameof(fileSearcher));
        }

        public async Task ExecuteAsync(UpdateProjectFilesCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default(CancellationToken))
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
        /// when <see cref="UpdateProjectFilesCommandHandlerImplementation"/> is handling a <see cref="UpdateProjectFilesCommand"/>
        /// </summary>
        private class ProgressCommandExecution : IFileSystem, IFileSearch, IDisposable
        {
            [NotNull]
            private readonly IFileSystem filesystem;

            [NotNull]
            private readonly IFileSearch fileSearch;

            [NotNull]
            private readonly IProgress<ProgressData> progress;

            private int foundFileCount;
            private int currentIndex;

            internal ProgressCommandExecution([NotNull] IFileSystem filesystem, [NotNull] IFileSearch fileSearch, [NotNull] IProgress<ProgressData> progress)
            {
                this.filesystem = filesystem ?? throw new ArgumentNullException(nameof(filesystem));
                this.fileSearch = fileSearch ?? throw new ArgumentNullException(nameof(fileSearch));
                this.progress = progress ?? throw new ArgumentNullException(nameof(progress));
                foundFileCount = 0;
            }

            public void Dispose()
            {
                progress.Report(new ProgressData("Done"));
            }

            bool IFileSystem.FileExists(string filename) => filesystem.FileExists(filename);

            Stream IFileSystem.ReadFile(string filename) => filesystem.ReadFile(filename);

            string IFileSystem.GetFileContent(string filename)
            {
                currentIndex++;
                progress.Report(new ProgressData(currentIndex, foundFileCount, $"Processing '{filename}'"));
                return filesystem.GetFileContent(filename);
            }

            void IFileSystem.SaveContent(string filename, string content)
            {
                progress.Report(new ProgressData(currentIndex, foundFileCount, $"File {filename} updated"));
                filesystem.SaveContent(filename, content);
            }

            Task IFileSystem.SaveContentAsync(string filename, Stream content) => filesystem.SaveContentAsync(filename, content);

            void IFileSystem.DeleteFile(string filename) => filesystem.DeleteFile(filename);

            string[] IFileSearch.FindFilesIncludingSubdirectories(string rootPath, string mask)
            {
                progress.Report(new ProgressData($"Find cs files within directory {rootPath}"));

                var result = fileSearch.FindFilesIncludingSubdirectories(rootPath, mask);

                foundFileCount = result.Length;
                currentIndex = 0;

                if (foundFileCount == 0)
                    progress.Report(new ProgressData("No files found"));

                return result;
            }
        }
    }
}
