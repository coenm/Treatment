namespace TestAgentEventsLogger.Interface.Control
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class StartSutRequest : IRequest
    {
        public string Executable { get; set; }
    }
}
