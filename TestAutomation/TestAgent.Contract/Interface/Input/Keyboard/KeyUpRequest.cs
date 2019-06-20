namespace TestAgent.Contract.Interface.Input.Keyboard
{
    using JetBrains.Annotations;
    using TestAgent.Contract.Interface.Input.Enums;

    /// <summary>
    /// Generate key(s) up.
    /// </summary>
    [PublicAPI]
    public class KeyUpRequest : IControlRequest
    {
        public VirtualKeyCode[] KeyCodes { get; set; }
    }
}
