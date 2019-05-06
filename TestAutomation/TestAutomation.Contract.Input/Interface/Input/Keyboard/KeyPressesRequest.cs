namespace TestAutomation.Contract.Input.Interface.Input.Keyboard
{
    using Enums;
    using JetBrains.Annotations;

    /// <summary>
    /// Generate key press(es)
    /// </summary>
    [PublicAPI]
    public class KeyPressesRequest : IRequest
    {
        public VirtualKeyCode[] KeyCodes { get; set; }
    }
}
