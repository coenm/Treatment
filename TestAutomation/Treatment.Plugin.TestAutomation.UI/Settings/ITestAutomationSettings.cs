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
    }
}
