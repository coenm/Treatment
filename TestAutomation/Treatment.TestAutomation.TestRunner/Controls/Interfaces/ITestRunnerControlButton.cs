﻿namespace Treatment.TestAutomation.TestRunner.Controls.Interfaces
{
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;

    internal interface ITestRunnerControlButton :
        IButton,
        IButtonClicked,
        IPositionUpdated,
        IIsEnabledChanged,
        ISizeUpdated,
        IFocusChange,
        IKeyboardFocusChanged,
        IControl,
        ITestRunnerControlPositionable,
        ITestRunnerControlFocusable
    {
    }
}
