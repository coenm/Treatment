namespace Treatment.TestAutomation.Contract.Interfaces.Events.Collection
{
    using System.Collections.Specialized;

    using JetBrains.Annotations;

    [PublicAPI]
    public class CollectionSelectionChanged : TestElementEventBase
    {
        public int? AddedCount { get; set; }
    }
}
