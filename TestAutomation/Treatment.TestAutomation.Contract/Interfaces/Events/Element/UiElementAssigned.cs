namespace Treatment.TestAutomation.Contract.Interfaces.Events.Element
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Assignment of a UI element to a property of a view or property of a container element.
    /// </summary>
    [PublicAPI]
    public class UiElementAssigned : TestElementEventBase
    {
        public string PropertyName { get; set; }

        public Guid ChildElement { get; set; }
    }
}
