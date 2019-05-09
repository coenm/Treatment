namespace Treatment.TestAutomation.Contract.Interfaces.Events.Collection
{
    using System.Collections.Specialized;

    using JetBrains.Annotations;

    [PublicAPI]
    public class SelectionChanged : TestElementEventBase
    {
        public int? AddedCount { get; set; }
    }

    [PublicAPI]
    public class CurrentChanged: TestElementEventBase
    {
    }

    [PublicAPI]
    public class DataContextChanged: TestElementEventBase
    {
    }

    [PublicAPI]
    public class CollectionChanged : TestElementEventBase
    {
        public string Action { get; set; }
    }
}
