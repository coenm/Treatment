namespace TestAgent.Contract.Interface.Input.Mouse
{
    using JetBrains.Annotations;
    using TestAgent.Contract.Interface.Base;

    [PublicAPI]
    public class MoveMouseToRequest : IControlRequest
    {
        public Point Position { get; set; }
    }
}
