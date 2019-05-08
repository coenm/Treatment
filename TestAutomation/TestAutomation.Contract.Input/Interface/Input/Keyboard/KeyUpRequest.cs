namespace TestAutomation.Input.Contract.Interface.Input.Keyboard
{
    using Enums;
    using JetBrains.Annotations;

    /// <summary>
    /// Generate key(s) up
    /// </summary>
    [PublicAPI]
    public class KeyUpRequest : IInputRequest
    {
        public VirtualKeyCode[] KeyCodes { get; set; }
    }
}
