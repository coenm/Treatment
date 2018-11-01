namespace Treatment.UI.Core.Configuration
{
    using System.IO;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Treatment.Core.Interfaces;
    using Treatment.Helpers;

    public class FileBasedConfigurationService : IConfigurationService
    {
        [NotNull] private readonly IFileSystem fileSystem;

        public FileBasedConfigurationService([NotNull] IFileSystem fileSystem)
        {
            this.fileSystem = Guard.NotNull(fileSystem, nameof(fileSystem));
        }

        public async Task<ApplicationSettings> GetAsync()
        {
            using (var fileStream = fileSystem.OpenRead(@"large.json", true))
            using (var streamReader = new StreamReader(fileStream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                var largeJson = await JArray.LoadAsync(jsonTextReader).ConfigureAwait(false);
                return largeJson.ToObject<ApplicationSettings>();
            }
        }

        public async Task<bool> UpdateAsync(ApplicationSettings configuration)
        {
            var json = JArray.FromObject(configuration);

            using (var fileStream = fileSystem.OpenWrite(@"large.json", true))
            using (var streamWriter = new StreamWriter(fileStream))
            using (var jsonTextWriter = new JsonTextWriter(streamWriter))
            {
                await json.WriteToAsync(jsonTextWriter).ConfigureAwait(false);
            }

            return true;
        }
    }
}
