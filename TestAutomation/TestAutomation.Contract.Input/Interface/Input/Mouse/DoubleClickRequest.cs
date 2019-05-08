namespace TestAutomation.Input.Contract.Interface.Input.Mouse
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class DoubleClickRequest : IInputRequest
    {
        public MouseButtons Button { get; set; }
    }
}
