namespace Treatment.TestAutomation.Contract.Interfaces.Events.Collection
{
    using System.Collections.Specialized;

    using JetBrains.Annotations;

    [PublicAPI]
    public class SelectionChanged : TestElementEventBase
    {
        public int? AddedCount { get; set; }
    }
}
