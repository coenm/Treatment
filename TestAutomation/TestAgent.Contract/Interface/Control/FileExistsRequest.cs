namespace TestAgent.Contract.Interface.Control
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class FileExistsRequest : IControlRequest
    {
        public string Filename { get; set; }
    }
}