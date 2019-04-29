namespace TestAgent.Interface.Input.Mouse
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class SingleClickRequest : IRequest
    {
        public MouseButtons Button { get; set; }
    }
}
