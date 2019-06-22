namespace TestAgent.Contract.Interface.Input.Keyboard
{
    using JetBrains.Annotations;
    using TestAgent.Contract.Interface.Input.Enums;

    /// <summary>
    /// Generate a key combination press(es).
    /// </summary>
    [PublicAPI]
    public class KeyCombinationPressRequest : IControlRequest
    {
        public VirtualKeyCode[] KeyCodes { get; set; }
    }
}
