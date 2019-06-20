namespace TestAutomation.Input.Contract.Interface
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class OkInputResponse : IInputResponse
    {
        [CanBeNull] public string Msg { get; set; }
    }
}
