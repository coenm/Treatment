namespace Treatment.TestAutomation.Contract.Interfaces.Events.Element
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class TextValueChanged : TestElementEventBase
    {
        public string Text { get; set; }
    }
}
