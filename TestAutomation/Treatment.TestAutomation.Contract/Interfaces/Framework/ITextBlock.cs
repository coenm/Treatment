namespace Treatment.TestAutomation.Contract.Interfaces.Framework
{
    using System;

    using Contract.Interfaces.Events.Element;

    public interface ITextBlock
    {
        event EventHandler<PositionUpdated> PositionUpdated;

        event EventHandler<SizeUpdated> SizeUpdated;

        event EventHandler<IsEnabledChanged> IsEnabledChanged;

        event EventHandler<TextValueChanged> TextValueChanged;
    }
}
