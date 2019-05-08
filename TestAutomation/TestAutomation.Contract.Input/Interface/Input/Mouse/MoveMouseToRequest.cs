namespace TestAutomation.Input.Contract.Interface.Input.Mouse
{
    using Base;
    using JetBrains.Annotations;

    [PublicAPI]
    public class MoveMouseToRequest : IInputRequest
    {
        public Point Position { get; set; }
    }
}
