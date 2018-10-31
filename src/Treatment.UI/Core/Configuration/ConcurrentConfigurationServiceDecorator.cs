namespace Treatment.UI.Core.Configuration
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Treatment.Helpers;

    public class ConcurrentConfigurationServiceDecorator : IConfigurationService
    {
        [NotNull]
        private readonly IConfigurationService decoratee;

        public ConcurrentConfigurationServiceDecorator([NotNull] IConfigurationService decoratee)
        {
            this.decoratee = Guard.NotNull(decoratee, nameof(decoratee));
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
