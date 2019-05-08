namespace TestAutomation.Input.Contract.Interface.Input.Keyboard
{
    using Enums;
    using JetBrains.Annotations;

    /// <summary>
    /// Generate key press(es)
    /// </summary>
    [PublicAPI]
    public class KeyPressesRequest : IInputRequest
    {
        public VirtualKeyCode[] KeyCodes { get; set; }
    }
}
