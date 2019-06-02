namespace TestAgent.Contract.Interface.Control
{
    public class GetFileResponse : IControlResponse
    {
        public byte[] Data { get; set; }
    }
}
