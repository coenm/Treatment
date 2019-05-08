namespace Treatment.TestAutomation.Contract.Interfaces.Events
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class NewControlCreated : TestElementEventBase
    {
        public string InterfaceType { get; set; }
    }
}
