namespace TestAutomation.Contract.Input.Interface.Input.Mouse
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class DoubleClickRequest : IRequest
    {
        public MouseButtons Button { get; set; }
    }
}
