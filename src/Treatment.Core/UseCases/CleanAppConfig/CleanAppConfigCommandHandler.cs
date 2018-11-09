namespace Treatment.Core.UseCases.CleanAppConfig
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Treatment.Contract;
    using Treatment.Contract.Commands;
    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.Contract.Plugin.SourceControl;
    using Treatment.Helpers.Guards;

    [UsedImplicitly]
    public class CleanAppConfigCommandHandler : ICommandHandler<CleanAppConfigCommand>
    {
        private readonly IFileSearch fileSearcher;
        private readonly IReadOnlySourceControl sourceControl;
        private readonly ICleanSingleAppConfig cleanSingleAppConfig;

        public CleanAppConfigCommandHandler(
            [NotNull] IFileSearch fileSearcher,
            [NotNull] IReadOnlySourceControl sourceControl,
            [NotNull] ICleanSingleAppConfig cleanSingleAppConfig)
        {
            Guard.NotNull(fileSearcher, nameof(fileSearcher));
            Guard.NotNull(sourceControl, nameof(sourceControl));
            Guard.NotNull(cleanSingleAppConfig, nameof(cleanSingleAppConfig));

            this.fileSearcher = fileSearcher;
            this.sourceControl = sourceControl;
            this.cleanSingleAppConfig = cleanSingleAppConfig;
        }

        public async Task ExecuteAsync(CleanAppConfigCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default)
        {
            Guard.NotNull(command, nameof(command));

            progress?.Report(new ProgressData(-1, -1, "Finding csharp files."));
            var projectFiles = GetCsFiles(command.Directory).ToArray();

            progress?.Report(new ProgressData(-1, -1, "Finding app.config files."));
            var appConfigFiles = GetAppConfigFiles(command.Directory);

            var count = projectFiles.Length;
            var index = 0;

            foreach (var projectFile in projectFiles)
            {
                progress?.Report(new ProgressData(index, count, $"Processing {projectFile}."));
                index++;

                var path = Path.GetDirectoryName(projectFile);

                var appConfigFile = appConfigFiles.SingleOrDefault(file => Path.GetDirectoryName(file) == path);
                if (appConfigFile == null)
                    continue;

                await HandleProjectFileAsync(projectFile, appConfigFile).ConfigureAwait(false);
            }

            progress?.Report(new ProgressData(count, count, "Done."));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task HandleProjectFileAsync(string projectFile, string appConfigFile)
        {
            var status = sourceControl.GetFileStatus(projectFile);
            if (status != FileStatus.New && status != FileStatus.Modified)
                return;

            status = sourceControl.GetFileStatus(appConfigFile);
            if (status != FileStatus.New)
                return;

            await cleanSingleAppConfig.ExecuteAsync(projectFile, appConfigFile).ConfigureAwait(false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<string> GetCsFiles(string rootPath)
        {
            return fileSearcher.FindFilesIncludingSubdirectories(rootPath, "*.csproj");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string[] GetAppConfigFiles(string rootPath)
        {
            var lowerCaseResult = fileSearcher.FindFilesIncludingSubdirectories(rootPath, "app.config");
            var uppercaseResult = fileSearcher.FindFilesIncludingSubdirectories(rootPath, "App.config");
            return lowerCaseResult.Concat(uppercaseResult)
                          .Distinct()
                          .ToArray();
        }
    }
}
