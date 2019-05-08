namespace TestAgent.Contract.Interface.Control
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class LocateFilesRequest : IControlRequest
    {
        public string Directory { get; set; }

        public string Filename { get; set; }
    }
}
