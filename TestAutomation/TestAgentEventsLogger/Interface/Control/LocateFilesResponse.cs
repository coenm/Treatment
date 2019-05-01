namespace TestAgentEventsLogger.Interface.Control
{
    using System.Collections.Generic;

    public class LocateFilesResponse : IResponse
    {
        public List<string> Executable { get; set; }
    }
}
