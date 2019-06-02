namespace Treatment.TestAutomation.Contract.Interfaces.Events.Element
{
    using System.Windows;

    using JetBrains.Annotations;

    [PublicAPI]
    public class SizeUpdated : TestElementEventBase
    {
        public Size Size { get; set; }
    }
}
