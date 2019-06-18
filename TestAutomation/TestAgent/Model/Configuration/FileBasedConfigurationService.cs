namespace TestAgent.Model.Configuration
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NLog;
    using Treatment.Helpers.FileSystem;
    using Treatment.Helpers.Guards;

    internal class FileBasedConfigurationService : IConfigurationService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        [NotNull] private readonly IConfigFilenameProvider filenameProvider;
        [NotNull] private readonly IResolveSutExecutable resolveSutExecutable;
        [NotNull] private readonly IFileSystem fileSystem;

        public FileBasedConfigurationService(
            [NotNull] IFileSystem fileSystem,
            [NotNull] IConfigFilenameProvider filenameProvider,
            [NotNull] IResolveSutExecutable resolveSutExecutable)
        {
            Guard.NotNull(fileSystem, nameof(fileSystem));
            Guard.NotNull(filenameProvider, nameof(filenameProvider));
            Guard.NotNull(resolveSutExecutable, nameof(resolveSutExecutable));

            this.filenameProvider = filenameProvider;
            this.resolveSutExecutable = resolveSutExecutable;
            this.fileSystem = fileSystem;
        }

        public async Task<TestAgentApplicationSettings> GetAsync()
        {
            var filename = filenameProvider.Filename;
            if (!fileSystem.FileExists(filename))
                return CreateDefaultSettings();

            try
            {
                using (var fileStream = fileSystem.OpenRead(filename, true))
                using (var streamReader = new StreamReader(fileStream))
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    var largeJson = await JObject.LoadAsync(jsonTextReader).ConfigureAwait(false);
                    var result = largeJson.ToObject<ApplicationSettingsDto>();
                    return Map(result);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Could not get or read file {filename}. Work with default settings.");
                return CreateDefaultSettings();
            }
        }

        public async Task<bool> UpdateAsync([NotNull] TestAgentApplicationSettings configuration)
        {
            Guard.NotNull(configuration, nameof(configuration));

            var configToSave = Map(configuration);

            JObject json;
            try
            {
                json = JObject.FromObject(configToSave);
            }
            catch (Exception e)
            {
                // todo
                throw e;
            }

            try
            {
                // todo check if directory exists..
                using (var fileStream = fileSystem.OpenWrite(filenameProvider.Filename, true))
                using (var streamWriter = new StreamWriter(fileStream))
                using (var jsonTextWriter = new JsonTextWriter(streamWriter))
                {
                    await json.WriteToAsync(jsonTextWriter).ConfigureAwait(false);
                }

                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Something wrong happened.");
                return false;
            }
        }

        private static ApplicationSettingsDto Map([NotNull] TestAgentApplicationSettings settings)
        {
            DebugGuard.NotNull(settings, nameof(settings));

            return new ApplicationSettingsDto
            {
                Executable = settings.Executable,
            };
        }

        [NotNull] private static TestAgentApplicationSettings Map(ApplicationSettingsDto settings)
        {
            return new TestAgentApplicationSettings
            {
                Executable = settings.Executable,
            };
        }

        private TestAgentApplicationSettings CreateDefaultSettings()
        {
            return new TestAgentApplicationSettings
            {
                Executable = resolveSutExecutable.Executable ?? string.Empty,
            };
        }

        private struct ApplicationSettingsDto
        {
            public string Executable { get; set; }
        }
    }
}
