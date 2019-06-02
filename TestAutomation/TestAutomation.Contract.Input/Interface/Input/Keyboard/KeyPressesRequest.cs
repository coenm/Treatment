namespace TestAutomation.Input.Contract.Interface.Input.Keyboard
{
    using JetBrains.Annotations;
    using TestAutomation.Input.Contract.Interface.Input.Enums;

    /// <summary>
    /// Generate key press(es).
    /// </summary>
    [PublicAPI]
    public class KeyPressesRequest : IInputRequest
    {
        public VirtualKeyCode[] KeyCodes { get; set; }
    }
}
