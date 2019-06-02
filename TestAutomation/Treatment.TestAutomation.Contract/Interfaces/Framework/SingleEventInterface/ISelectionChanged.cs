namespace Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface
{
    using System;

    using Events.Element;

    public interface ISelectionChanged
    {
        event EventHandler<SelectionChanged> SelectionChanged;
    }
}
