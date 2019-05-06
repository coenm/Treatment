namespace TestAutomation.Contract.Input.Interface.Input.Keyboard
{
    using Enums;
    using JetBrains.Annotations;

    /// <summary>
    /// Generate key down
    /// </summary>
    [PublicAPI]
    public class KeyDownRequest : IRequest
    {
        public VirtualKeyCode[] KeyCodes { get; set; }
    }
}
