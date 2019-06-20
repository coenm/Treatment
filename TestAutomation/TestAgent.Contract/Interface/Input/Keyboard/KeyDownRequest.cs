namespace TestAgent.Contract.Interface.Input.Keyboard
{
    using JetBrains.Annotations;
    using TestAgent.Contract.Interface.Input.Enums;

    /// <summary>
    /// Generate key down.
    /// </summary>
    [PublicAPI]
    public class KeyDownRequest : IControlRequest
    {
        public VirtualKeyCode[] KeyCodes { get; set; }
    }
}
