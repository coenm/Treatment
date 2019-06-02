namespace TestAgent.Contract.Interface
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class ExceptionResponse : IControlResponse
    {
        public string Message { get; set; }
    }
}
