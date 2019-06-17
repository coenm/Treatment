namespace Treatment.TestAutomation.TestRunner.Framework.Settings
{
    using System;
    using System.Configuration;

    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;

    internal class AppConfigSutSettings : ISutSettings
    {
        public string SutExecutable
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings.Get("SutExecutable");
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public string WorkingDirectory
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings.Get("WorkingDirectory");
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
