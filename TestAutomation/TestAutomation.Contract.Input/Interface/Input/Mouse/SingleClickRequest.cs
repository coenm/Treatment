namespace TestAutomation.Contract.Input.Interface.Input.Mouse
{
    using Base;
    using JetBrains.Annotations;

    [PublicAPI]
    public class SingleClickRequest : IRequest
    {
        public MouseButtons Button { get; set; }

        public Point Position { get; set; }
    }
}
