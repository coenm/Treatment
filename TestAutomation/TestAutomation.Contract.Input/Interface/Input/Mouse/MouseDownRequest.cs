namespace TestAutomation.Input.Contract.Interface.Input.Mouse
{
    using JetBrains.Annotations;
    using TestAutomation.Input.Contract.Interface.Base;

    [PublicAPI]
    public class MouseDownRequest : IInputRequest
    {
        public MouseButtons Button { get; set; }

        public Point Position { get; set; }
    }
}
