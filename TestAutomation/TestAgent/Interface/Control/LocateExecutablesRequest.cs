namespace TestAgent.Interface.Control
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class LocateExecutablesRequest : IRequest
    {
        public string Directory { get; set; }

        public string Filename { get; set; }
    }
}
