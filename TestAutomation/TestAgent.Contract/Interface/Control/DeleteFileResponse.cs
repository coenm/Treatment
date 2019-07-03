namespace TestAgent.Contract.Interface.Control
{
    public class DeleteFileResponse : IControlResponse
    {
        public bool Deleted { get; set; }
    }
}