﻿namespace Treatment.Plugin.TestAutomation.UI.Settings
{
    internal class DummySettings : ITestAutomationSettings
    {
        public bool TestAutomationEnabled => true;

        public string ZeroMqEventPublishSocket => "inproc://a";

        public string ZeroMqKey => "abc";
    }
}
