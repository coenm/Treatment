namespace TestAgentEventsLogger.Interface.Input.Keyboard
{
    using JetBrains.Annotations;

    using TestAgentEventsLogger.Interface.Input.Enums;

    /// <summary>
    /// Generate key(s) up
    /// </summary>
    [PublicAPI]
    public class KeyUpRequest : IRequest
    {
        public VirtualKeyCode[] KeyCodes { get; set; }
    }
}
