﻿namespace Treatment.TestAutomation.TestRunner.Framework.Settings
{
    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;

    internal class StaticAgentSettings : IAgentSettings
    {
        public StaticAgentSettings([NotNull] string eventsEndpoint, [NotNull] string controlEndpoint)
        {
            Guard.NotNullOrWhiteSpace(eventsEndpoint, nameof(eventsEndpoint));
            Guard.NotNullOrWhiteSpace(controlEndpoint, nameof(controlEndpoint));

            EventsEndpoint = eventsEndpoint;
            ControlEndpoint = controlEndpoint;
        }

        public string EventsEndpoint { get; }

        public string ControlEndpoint { get; }
    }
}
