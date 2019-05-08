namespace TestAutomation.Input.Contract.Interface.Input.Keyboard
{
    using Enums;
    using JetBrains.Annotations;

    /// <summary>
    /// Generate key down
    /// </summary>
    [PublicAPI]
    public class KeyDownRequest : IInputRequest
    {
        public VirtualKeyCode[] KeyCodes { get; set; }
    }
}
