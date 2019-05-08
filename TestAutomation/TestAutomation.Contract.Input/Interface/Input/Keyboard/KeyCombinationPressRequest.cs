namespace TestAutomation.Input.Contract.Interface.Input.Keyboard
{
    using Enums;
    using JetBrains.Annotations;

    /// <summary>
    /// Generate a key combination press(es)
    /// </summary>
    [PublicAPI]
    public class KeyCombinationPressRequest : IInputRequest
    {
        public VirtualKeyCode[] KeyCodes { get; set; }
    }
}
