namespace Treatment.Core.UseCases.CleanAppConfig
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Treatment.Contract;
    using Treatment.Contract.Commands;
    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.Contract.Plugin.SourceControl;

    [UsedImplicitly]
    public class CleanAppConfigCommandHandler : ICommandHandler<CleanAppConfigCommand>
    {
        private readonly IFileSearch _fileSearcher;
        private readonly IReadOnlySourceControl _sourceControl;
        private readonly ICleanSingleAppConfig _cleanSingleAppConfig;

        public CleanAppConfigCommandHandler(
            [NotNull] IFileSearch fileSearcher,
            [NotNull] IReadOnlySourceControl sourceControl,
            [NotNull] ICleanSingleAppConfig cleanSingleAppConfig)
        {
            _fileSearcher = fileSearcher;
            _sourceControl = sourceControl;
            _cleanSingleAppConfig = cleanSingleAppConfig;
        }

        public async Task ExecuteAsync(CleanAppConfigCommand command)
        {
            var projectFiles = GetCsFiles(command.Directory);
            var appConfigFiles = GetAppConfigFiles(command.Directory);

            foreach (var projectFile in projectFiles)
            {
                var path = Path.GetDirectoryName(projectFile);

                var appConfigFile = appConfigFiles.SingleOrDefault(file => Path.GetDirectoryName(file) == path);
                if (appConfigFile == null)
                    continue;

                await HandleProjectFileAsync(projectFile, appConfigFile).ConfigureAwait(false);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task HandleProjectFileAsync(string projectFile, string appConfigFile)
        {
            var status = _sourceControl.GetFileStatus(projectFile);
            if (status != FileStatus.New && status != FileStatus.Modified)
                return;

            status = _sourceControl.GetFileStatus(appConfigFile);
            if (status != FileStatus.New)
                return;

            await _cleanSingleAppConfig.ExecuteAsync(projectFile, appConfigFile).ConfigureAwait(false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<string> GetCsFiles(string rootPath)
        {
            return _fileSearcher.FindFilesIncludingSubdirectories(rootPath, "*.csproj");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string[] GetAppConfigFiles(string rootPath)
        {
            var lowerCaseResult = _fileSearcher.FindFilesIncludingSubdirectories(rootPath, "app.config");
            var uppercaseResult = _fileSearcher.FindFilesIncludingSubdirectories(rootPath, "App.config");
            return lowerCaseResult.Concat(uppercaseResult)
                          .Distinct()
                          .ToArray();
        }
    }
}