namespace TestAgent.Contract.Interface.Control
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class StartSutRequest : IControlRequest
    {
        [CanBeNull]
        public string WorkingDirectory { get; set; }
    }
}
