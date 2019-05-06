namespace TestAutomation.Contract.Input.Interface
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class OkResponse : IResponse
    {
        [CanBeNull] public string Msg { get; set; }
    }
}
