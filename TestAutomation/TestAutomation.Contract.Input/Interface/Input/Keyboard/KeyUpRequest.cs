namespace TestAutomation.Input.Contract.Interface.Input.Keyboard
{
    using JetBrains.Annotations;
    using TestAutomation.Input.Contract.Interface.Input.Enums;

    /// <summary>
    /// Generate key(s) up.
    /// </summary>
    [PublicAPI]
    public class KeyUpRequest : IInputRequest
    {
        public VirtualKeyCode[] KeyCodes { get; set; }
    }
}
