namespace TestAgent.Interface.Control
{
    using System.Collections.Generic;

    public class LocateExecutablesResponse : IResponse
    {
        public List<string> Executable { get; set; }
    }
}
