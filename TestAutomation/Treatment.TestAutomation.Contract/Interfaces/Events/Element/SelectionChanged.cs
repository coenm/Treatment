namespace Treatment.TestAutomation.Contract.Interfaces.Events.Element
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class SelectionChanged : TestElementEventBase
    {
        [CanBeNull]
        public string SelectedItem { get; set; }
    }
}
