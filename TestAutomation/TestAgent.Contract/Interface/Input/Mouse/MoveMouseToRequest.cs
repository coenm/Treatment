namespace TestAutomation.Input.Contract.Interface.Input.Mouse
{
    using JetBrains.Annotations;
    using TestAutomation.Input.Contract.Interface.Base;

    [PublicAPI]
    public class MoveMouseToRequest : IInputRequest
    {
        public Point Position { get; set; }
    }
}
