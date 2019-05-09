namespace TestAgent.Contract.Interface.Control
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class GetFileRequest : IControlRequest
    {
        public string Filename { get; set; }
    }
}
