namespace TestAgent.Contract.Interface.Input.Mouse
{
    using JetBrains.Annotations;
    using TestAgent.Contract.Interface.Base;

    [PublicAPI]
    public class MouseUpRequest : IControlRequest
    {
        public MouseButtons Button { get; set; }

        public Point Position { get; set; }
    }
}
