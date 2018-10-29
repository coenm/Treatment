namespace Treatment.UI.Core.Configuration
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using SimpleInjector;
    using Treatment.Core.Interfaces;

    public class FileBasedConfigurationService : IConfigurationService
    {
        [NotNull] private readonly IFileSystem fileSystem; //todo
        [NotNull] private readonly Container container;

        public FileBasedConfigurationService([NotNull] IFileSystem fileSystem, [NotNull] Container container)
        {
            this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            this.container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public async Task<ApplicationSettings> GetAsync()
        {
            // read asynchronously from a file
            // todo WIP
            using (var asyncFileStream = new FileStream(@"large.json", FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                var largeJson = await JArray.LoadAsync(new JsonTextReader(new StreamReader(asyncFileStream))).ConfigureAwait(false);

                return largeJson.ToObject<ApplicationSettings>();
            }
        }

        public async Task<bool> UpdateAsync(ApplicationSettings configuration)
        {
            var json = JArray.FromObject(configuration);

            // write asynchronously to a file
            using (var asyncFileStream = new FileStream(@"large.json", FileMode.Open, FileAccess.Write, FileShare.Write, 4096, true))
            {
                await json.WriteToAsync(new JsonTextWriter(new StreamWriter(asyncFileStream))).ConfigureAwait(false);
            }

            return true;
        }

        public IConfiguration GetConfiguration() => container.GetInstance<IConfiguration>();
    }
}
