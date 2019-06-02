namespace TestAgent.Contract.Interface.Control
{
    public class StartSutResponse : IControlResponse
    {
        public bool Success { get; set; }

        public string Executable { get; set; }
    }
}
