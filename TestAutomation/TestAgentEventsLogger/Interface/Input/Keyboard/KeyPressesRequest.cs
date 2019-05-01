namespace TestAgentEventsLogger.Interface.Input.Keyboard
{
    using JetBrains.Annotations;

    using TestAgentEventsLogger.Interface.Input.Enums;

    /// <summary>
    /// Generate key press(es)
    /// </summary>
    [PublicAPI]
    public class KeyPressesRequest : IRequest
    {
        public VirtualKeyCode[] KeyCodes { get; set; }
    }
}
