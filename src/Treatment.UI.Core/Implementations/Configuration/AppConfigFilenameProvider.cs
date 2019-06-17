namespace Treatment.UI.Core.Implementations.Configuration
{
    using System;
    using System.Configuration;

    internal class AppConfigFilenameProvider : IConfigFilenameProvider
    {
        public string Filename
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings.Get("ConfigFilename");
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }
    }
}
