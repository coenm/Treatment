namespace Treatment.TestAutomation.Contract.Interfaces.Events.Collection
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class CollectionChanged : TestElementEventBase
    {
        public string Action { get; set; }
    }
}