﻿namespace Treatment.TestAutomation.Contract.Interfaces.Application
{
    using global::Treatment.TestAutomation.Contract.Interfaces.Framework;

    public interface IMainViewStatusBar : IStatusBar
    {
        ITextBlock StatusText { get; }

        ITextBlock StatusConfigFilename { get; }

        ITextBlock StatusDelayProcessCounter { get; }
    }
}
