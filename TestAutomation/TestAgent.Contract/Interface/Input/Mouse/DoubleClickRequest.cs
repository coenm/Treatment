namespace TestAgent.Contract.Interface.Input.Mouse
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class DoubleClickRequest : IControlRequest
    {
        public MouseButtons Button { get; set; }
    }
}
