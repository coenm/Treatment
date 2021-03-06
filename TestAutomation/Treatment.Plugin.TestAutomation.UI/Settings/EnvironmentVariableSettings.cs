﻿namespace Treatment.Plugin.TestAutomation.UI.Settings
{
    using System;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;

    internal class EnvironmentVariableSettings : ITestAutomationSettings
    {
        public bool TestAutomationEnabled
        {
            get
            {
                if (TryGetBool("ENABLE_TEST_AUTOMATION", out var value))
                    return value;
                return false;
            }
        }

        [CanBeNull]
        public string ZeroMqEventPublishSocket { get; } = GetString("TA_PUBLISH_SOCKET");

        private static bool TryGetBool(string key, out bool value)
        {
            Guard.NotNull(key, nameof(key));

            value = false;

            try
            {
                var @string = Environment.GetEnvironmentVariable(key);

                if (string.IsNullOrWhiteSpace(@string))
                    return false;

                return bool.TryParse(@string, out value);
            }
            catch (Exception)
            {
                return false;
            }
        }

        [CanBeNull]
        private static string GetString([NotNull] string key)
        {
            Guard.NotNull(key, nameof(key));
            try
            {
                return Environment.GetEnvironmentVariable(key);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
