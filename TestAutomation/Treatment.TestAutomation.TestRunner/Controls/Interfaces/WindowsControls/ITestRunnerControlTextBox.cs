﻿namespace Treatment.TestAutomation.TestRunner.Controls.Interfaces.WindowsControls
{
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;

    internal interface ITestRunnerControlTextBox :
        ITextBox,
        IPositionUpdated,
        IIsEnabledChanged,
        ISizeUpdated,
        IFocusChange,
        IKeyboardFocusChanged,
        ITextValueChanged,
        IControl,
        IElementLoadedUnLoaded,
        ITestRunnerControlPositionable,
        ITestRunnerControlFocusable
    {
    }
}
