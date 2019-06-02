namespace TestAgent.Contract.Interface.Control
{
    using System.Collections.Generic;

    public class LocateFilesResponse : IControlResponse
    {
        public List<string> Executable { get; set; }
    }
}
