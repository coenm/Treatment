namespace TestAgentEventsLogger.Interface.Input.Keyboard
{
    using JetBrains.Annotations;

    using TestAgentEventsLogger.Interface.Input.Enums;

    /// <summary>
    /// Generate key down
    /// </summary>
    [PublicAPI]
    public class KeyDownRequest : IRequest
    {
        public VirtualKeyCode[] KeyCodes { get; set; }
    }
}
