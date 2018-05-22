namespace Treatment.Core.UseCases.CleanAppConfig
{
    using System.IO;

    using JetBrains.Annotations;

    using Treatment.Core.Interfaces;

    [UsedImplicitly]
    public class CleanSingleAppConfig : ICleanSingleAppConfig
    {
        private readonly IFileSystem _filesystem;

        public CleanSingleAppConfig([NotNull] IFileSystem filesystem)
        {
            _filesystem = filesystem;
        }

        // no verification anymore. just execute
        public bool Execute(string projectFile, string appConfigFile)
        {
            if (!RemoveAppConfigFromProjectFile(projectFile))
                return false;

            _filesystem.DeleteFile(appConfigFile);
            return true;
        }

        private bool RemoveAppConfigFromProjectFile(string projectFile)
        {
            CSharpProjectFileUpdater csProjFile;
            using (var readFile = _filesystem.ReadFile(projectFile))
            {
                csProjFile = CSharpProjectFileUpdater.Create(readFile);

                csProjFile.RemoveAppConfig();

                if (!csProjFile.HasChanges)
                    return false;

                csProjFile.RemoveEmptyItemGroups();
            }

            using (var outputStream = new MemoryStream())
            {
                csProjFile.Save(outputStream);
                outputStream.Position = 0;
                _filesystem.SaveContent(projectFile, outputStream);
            }

            return true;
        }
    }
}