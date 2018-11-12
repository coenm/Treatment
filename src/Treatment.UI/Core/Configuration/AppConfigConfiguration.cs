namespace Treatment.UI.Core.Configuration
{
    using System;
    using System.Configuration;

    using Treatment.Core.DefaultPluginImplementation.FileSearch;

    internal class AppConfigConfiguration : ISearchProviderNameOption
    {
        public string SearchProviderName
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings.Get("DefaultSearchProviderName");
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
