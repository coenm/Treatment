namespace Treatment.UI.Core.Configuration
{
    using System;
    using System.Threading.Tasks;

    public interface IReadOnlyConfigurationService
    {
        IObservable<bool> ConfigurationChanged { get; }

        Task<ApplicationSettings> GetAsync();
    }
}
