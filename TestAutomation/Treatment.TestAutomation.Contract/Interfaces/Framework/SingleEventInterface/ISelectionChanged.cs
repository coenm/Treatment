﻿namespace Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface
{
    using System;

    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;

    public interface ISelectionChanged
    {
        event EventHandler<SelectionChanged> SelectionChanged;
    }
}
