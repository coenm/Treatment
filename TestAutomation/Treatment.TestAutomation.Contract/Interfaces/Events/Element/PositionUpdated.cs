namespace Treatment.TestAutomation.Contract.Interfaces.Events.Element
{
    using System.Windows;

    using JetBrains.Annotations;

    [PublicAPI]
    public class PositionUpdated : TestElementEventBase
    {
        public Point Point { get; set; }
    }
}
