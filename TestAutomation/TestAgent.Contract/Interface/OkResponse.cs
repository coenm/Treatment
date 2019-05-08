namespace TestAgent.Contract.Interface
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class OkResponse : IControlResponse
    {
        [CanBeNull] public string Msg { get; set; }
    }
}
