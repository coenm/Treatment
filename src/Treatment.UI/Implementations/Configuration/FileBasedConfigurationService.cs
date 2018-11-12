namespace Treatment.UI.Implementations.Configuration
{
    using System;
    using System.IO;
    using System.Reactive.Subjects;
    using System.Reflection;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Treatment.Core.Interfaces;
    using Treatment.Helpers.Guards;
    using Treatment.UI.Core.Configuration;

    internal class FileBasedConfigurationService : IConfigurationService
    {
        [NotNull] private readonly ISubject<bool> configurationChanged = new ReplaySubject<bool>(1);
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

        public IObservable<bool> ConfigurationChanged => configurationChanged/*.DistinctUntilChanged()*/;

        public async Task<ApplicationSettings> GetAsync()
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

        public async Task<bool> UpdateAsync([NotNull] ApplicationSettings configuration)
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

                configurationChanged.OnNext(true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static ApplicationSettings CreateDefaultSettings()
        {
            return new ApplicationSettings
            {
                DelayExecution = new DelayExecutionSettings
                                 {
                                     Enabled = false,
                                     MinMilliseconds = 0,
                                     MaxMilliseconds = 0,
                                 },
                RootDirectory = Assembly.GetExecutingAssembly().Location,
                SearchProviderName = string.Empty,
                VersionControlProviderName = string.Empty,
            };
        }

        private static ApplicationSettingsDto Map([NotNull] ApplicationSettings settings)
        {
            DebugGuard.NotNull(settings, nameof(settings));

            return new ApplicationSettingsDto
            {
                DelayExecution = new DelaySettingsDto
                                 {
                                     Enabled = settings.DelayExecution.Enabled,
                                     MinMilliseconds = settings.DelayExecution.MinMilliseconds,
                                     MaxMilliseconds = settings.DelayExecution.MaxMilliseconds,
                                 },
                RootDirectory = settings.RootDirectory,
                SearchProviderName = settings.SearchProviderName,
                VersionControlProviderName = settings.VersionControlProviderName,
            };
        }

        [NotNull] private static ApplicationSettings Map(ApplicationSettingsDto settings)
        {
            return new ApplicationSettings
            {
                DelayExecution = new DelayExecutionSettings
                                 {
                                     Enabled = settings.DelayExecution.Enabled,
                                     MinMilliseconds = settings.DelayExecution.MinMilliseconds,
                                     MaxMilliseconds = settings.DelayExecution.MaxMilliseconds,
                                 },
                RootDirectory = settings.RootDirectory,
                SearchProviderName = settings.SearchProviderName,
                VersionControlProviderName = settings.VersionControlProviderName,
            };
        }

        private struct ApplicationSettingsDto
        {
            public DelaySettingsDto DelayExecution { get; set; }

            public string SearchProviderName { get; set; }

            public string VersionControlProviderName { get; set; }

            public string RootDirectory { get; set; }
        }

        private struct DelaySettingsDto
        {
            public bool Enabled { get; set; }

            public int MinMilliseconds { get; set; }

            public int MaxMilliseconds { get; set; }
        }
    }
}
