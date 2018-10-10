namespace Treatment.UI.Core
{
    using System;
    using System.Configuration;

    using Treatment.Core.DefaultPluginImplementation.FileSearch;

    internal class AppConfigConfiguration : IConfiguration, ISearchProviderNameOption
    {
        public string RootPath
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings.Get("RootPath");
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public string SearchProviderName
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings.Get("SearchProviderName");
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}