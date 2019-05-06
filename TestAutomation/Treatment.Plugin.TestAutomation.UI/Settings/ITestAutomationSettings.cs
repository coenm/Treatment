namespace Treatment.Plugin.TestAutomation.UI.Settings
{
    using JetBrains.Annotations;

    internal interface ITestAutomationSettings
    {
        /// <summary>
        /// Boolean representing if TestAutomation should be enabled or not.
        /// </summary>
        bool TestAutomationEnabled { get; }

        /// <summary>
        /// ZeroMq event publishing endpoint.
        /// </summary>
        [CanBeNull]
        string ZeroMqEventPublishSocket { get; }

        /// <summary>
        /// ZeroMq endpoint for handling request to move mouse and keyboard.
        /// </summary>
        [CanBeNull]
        string ZeroMqRequestResponseSocket { get; }

        /// <summary>
        /// ZeroMq private key.
        /// </summary>
        [CanBeNull]
        string ZeroMqKey { get; }
    }
}
