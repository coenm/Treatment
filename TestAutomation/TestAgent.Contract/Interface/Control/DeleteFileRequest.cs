namespace TestAgent.Contract.Interface.Control
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class DeleteFileRequest : IControlRequest
    {
        public string Filename { get; set; }
    }
}
