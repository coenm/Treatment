namespace TestAutomation.Input.Contract.Interface.Input.Keyboard
{
    using JetBrains.Annotations;
    using TestAutomation.Input.Contract.Interface.Input.Enums;

    /// <summary>
    /// Generate a key combination press(es).
    /// </summary>
    [PublicAPI]
    public class KeyCombinationPressRequest : IInputRequest
    {
        public VirtualKeyCode[] KeyCodes { get; set; }
    }
}
