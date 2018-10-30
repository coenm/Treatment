namespace Treatment.UI.Core.Configuration
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    public class ConcurrentConfigurationServiceDecorator : IConfigurationService
    {
        [NotNull]
        private readonly IConfigurationService decoratee;

        public ConcurrentConfigurationServiceDecorator([NotNull] IConfigurationService decoratee)
        {
            this.decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        public Task<ApplicationSettings> GetAsync()
        {
            // todo
            return decoratee.GetAsync();
        }

        public Task<bool> UpdateAsync(ApplicationSettings configuration)
        {
            // todo
            return decoratee.UpdateAsync(configuration);
        }

        public IConfiguration GetConfiguration() => decoratee.GetConfiguration();
    }
}
