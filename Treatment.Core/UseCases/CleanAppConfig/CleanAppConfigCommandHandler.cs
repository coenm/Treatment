namespace Treatment.Core.UseCases.CleanAppConfig
{
    using System.IO;
    using System.Linq;

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

        public void Execute(CleanAppConfigCommand command)
        {
            var projectFiles = GetCsFiles(command.Directory);
            var appconfigFiles = GetAppConfigFiles(command.Directory);

            foreach (var projectFile in projectFiles)
            {
                var path = Path.GetDirectoryName(projectFile);

                var appConfigFile = appconfigFiles.SingleOrDefault(file => Path.GetDirectoryName(file) == path);
                if (appConfigFile == null)
                    continue;

                HandleProjectFile(projectFile, appConfigFile);
            }
        }

        private void HandleProjectFile(string projectFile, string appConfigFile)
        {
            var status = _sourceControl.GetFileStatus(projectFile);
            if (status != FileStatus.New && status != FileStatus.Modified)
                return;

            status = _sourceControl.GetFileStatus(appConfigFile);
            if (status != FileStatus.New)
                return;

            _cleanSingleAppConfig.Execute(projectFile, appConfigFile);
        }

        private string[] GetCsFiles(string rootpath)
        {
            return _fileSearcher.FindFilesIncludingSubdirectories(rootpath, "*.csproj");
        }

        private string[] GetAppConfigFiles(string rootpath)
        {
            var result1 = _fileSearcher.FindFilesIncludingSubdirectories(rootpath, "app.config");
            var result2 = _fileSearcher.FindFilesIncludingSubdirectories(rootpath, "App.config");
            return result1.Concat(result2)
                          .Distinct()
                          .ToArray();
        }
    }
}