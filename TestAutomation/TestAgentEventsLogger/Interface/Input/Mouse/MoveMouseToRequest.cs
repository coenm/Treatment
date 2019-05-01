namespace TestAgentEventsLogger.Interface.Input.Mouse
{
    using System.Windows;

    using JetBrains.Annotations;

    [PublicAPI]
    public class MoveMouseToRequest : IRequest
    {
        public Point Position { get; set; }
    }
}
