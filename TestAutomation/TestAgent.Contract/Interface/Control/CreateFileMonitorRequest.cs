namespace TestAgent.Contract.Interface.Control
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class CreateFileMonitorRequest : IControlRequest
    {
        public string Filename { get; set; }
    }
}