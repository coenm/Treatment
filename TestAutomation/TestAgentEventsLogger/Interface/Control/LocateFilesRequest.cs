namespace TestAgentEventsLogger.Interface.Control
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class LocateFilesRequest : IRequest
    {
        public string Directory { get; set; }

        public string Filename { get; set; }
    }
}
