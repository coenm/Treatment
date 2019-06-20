namespace TestAgent.Contract.Interface.Input.Keyboard
{
    using JetBrains.Annotations;
    using TestAgent.Contract.Interface.Input.Enums;

    /// <summary>
    /// Generate key press(es).
    /// </summary>
    [PublicAPI]
    public class KeyPressesRequest : IControlRequest
    {
        public VirtualKeyCode[] KeyCodes { get; set; }
    }
}
