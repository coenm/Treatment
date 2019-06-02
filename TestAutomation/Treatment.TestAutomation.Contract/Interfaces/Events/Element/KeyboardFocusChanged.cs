namespace Treatment.TestAutomation.Contract.Interfaces.Events.Element
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class KeyboardFocusChanged : TestElementEventBase
    {
        public bool Focussed { get; set; }
    }
}
