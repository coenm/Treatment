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
        public void Execute(string projectFile, string appConfigFile)
        {
            RemoveAppConfigFromProjectFile(projectFile);

            _filesystem.DeleteFile(appConfigFile);
        }

        private bool RemoveAppConfigFromProjectFile(string projectFile)
        {
            using (var readFile = _filesystem.ReadFile(projectFile))
            {
                var csProjFile = CSharpProjectFileUpdater
                                 .Create(readFile)
                                 .RemoveAppConfig()
                                 .RemoveEmptyItemGroups();

                if (!csProjFile.HasChanges)
                    return false;


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
}