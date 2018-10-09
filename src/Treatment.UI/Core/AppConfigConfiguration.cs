namespace Treatment.UI.Core
{
    using System;
    using System.Configuration;

    internal class AppConfigConfiguration : IConfiguration
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
    }
}