namespace TestAgent.Contract.Interface
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class OkInputResponse : IControlResponse
    {
        [CanBeNull] public string Msg { get; set; }
    }
}
