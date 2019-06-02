namespace Treatment.TestAutomation.TestRunner.Framework.Interfaces
{
    internal interface IAgentSettings
    {
        /// <summary>
        /// Full Endpoint to receive Agent and Sut events.
        /// </summary>
        string EventsEndpoint { get; }

        /// <summary>
        /// Full endpoint to send agent commands to.
        /// </summary>
        string ControlEndpoint { get; }
    }
}
