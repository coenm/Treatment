namespace Treatment.TestAutomation.Contract.Interfaces.Events.Element
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class IsEnabledChanged : TestElementEventBase
    {
        public bool Enabled { get; set; }
    }
}
