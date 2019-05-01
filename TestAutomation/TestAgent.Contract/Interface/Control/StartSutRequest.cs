namespace TestAgent.Contract.Interface.Control
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class StartSutRequest : IRequest
    {
        public string Executable { get; set; }
    }
}
