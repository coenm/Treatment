namespace Treatment.Core.UseCases.CleanAppConfig
{
    using System.IO;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Treatment.Helpers.FileSystem;

    [UsedImplicitly]
    public class CleanSingleAppConfig : ICleanSingleAppConfig
    {
        private readonly IFileSystem filesystem;

        public CleanSingleAppConfig([NotNull] IFileSystem filesystem)
        {
            this.filesystem = filesystem;
        }

        // no verification anymore. just execute
        public async Task<bool> ExecuteAsync(string projectFile, string appConfigFile)
        {
            var success = await RemoveAppConfigFromProjectFile(projectFile).ConfigureAwait(false);
            if (!success)
                return false;

            filesystem.DeleteFile(appConfigFile);
            return true;
        }

        private async Task<bool> RemoveAppConfigFromProjectFile(string projectFile)
        {
            CSharpProjectFileUpdater csProjFile;
            using (var readFile = filesystem.OpenRead(projectFile, false))
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
                await filesystem.SaveContentAsync(projectFile, outputStream).ConfigureAwait(false);
            }

            return true;
        }
    }
}
