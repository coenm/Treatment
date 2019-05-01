namespace TestAgentEventsLogger.Interface.Input.Keyboard
{
    using JetBrains.Annotations;

    using TestAgentEventsLogger.Interface.Input.Enums;

    /// <summary>
    /// Generate a key combination press(es)
    /// </summary>
    [PublicAPI]
    public class KeyCombinationPressRequest : IRequest
    {
        public VirtualKeyCode[] KeyCodes { get; set; }
    }
}
