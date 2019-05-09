namespace TestAutomation.Input.Contract.Interface.Input.Keyboard
{
    using JetBrains.Annotations;
    using TestAutomation.Input.Contract.Interface.Input.Enums;

    /// <summary>
    /// Generate key down.
    /// </summary>
    [PublicAPI]
    public class KeyDownRequest : IInputRequest
    {
        public VirtualKeyCode[] KeyCodes { get; set; }
    }
}
