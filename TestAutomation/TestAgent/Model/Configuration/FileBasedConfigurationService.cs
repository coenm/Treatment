namespace TestAgent.Model.Configuration
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Treatment.Helpers.FileSystem;
    using Treatment.Helpers.Guards;

    internal class FileBasedConfigurationService : IConfigurationService
    {
        [NotNull] private readonly IConfigFilenameProvider filenameProvider;
        [NotNull] private readonly IFileSystem fileSystem;

        public FileBasedConfigurationService(
            [NotNull] IFileSystem fileSystem,
            [NotNull] IConfigFilenameProvider filenameProvider)
        {
            Guard.NotNull(fileSystem, nameof(fileSystem));
            Guard.NotNull(filenameProvider, nameof(filenameProvider));

            this.filenameProvider = filenameProvider;
            this.fileSystem = fileSystem;
        }

        public async Task<TestAgentApplicationSettings> GetAsync()
        {
            if (!fileSystem.FileExists(filenameProvider.Filename))
                return CreateDefaultSettings();

            try
            {
                using (var fileStream = fileSystem.OpenRead(filenameProvider.Filename, true))
                using (var streamReader = new StreamReader(fileStream))
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    var largeJson = await JObject.LoadAsync(jsonTextReader).ConfigureAwait(false);
                    var result = largeJson.ToObject<ApplicationSettingsDto>();
                    return Map(result);
                }
            }
            catch (Exception)
            {
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
            catch (Exception)
            {
                return false;
            }
        }

        private static TestAgentApplicationSettings CreateDefaultSettings()
        {
            return new TestAgentApplicationSettings
            {
                Executable = Assembly.GetExecutingAssembly().Location,
            };
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

        private struct ApplicationSettingsDto
        {
            public string Executable { get; set; }
        }
    }
}
