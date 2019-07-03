namespace TestAgent.Contract.Interface.Control
{
    public class FileExistsResponse : IControlResponse
    {
        public bool FileExists { get; set; }
    }
}
