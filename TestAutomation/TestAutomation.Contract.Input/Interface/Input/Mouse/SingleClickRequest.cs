namespace TestAutomation.Input.Contract.Interface.Input.Mouse
{
    using Base;
    using JetBrains.Annotations;

    [PublicAPI]
    public class SingleClickRequest : IInputRequest
    {
        public MouseButtons Button { get; set; }

        public Point Position { get; set; }
    }
}
