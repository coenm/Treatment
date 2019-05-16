﻿namespace Treatment.TestAutomation.Contract.Interfaces.Treatment
{
    using Framework;

    using global::Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;

    public interface IMainWindow :
        IInitialized,
        IWindowClosing,
        IWindowClosed,
        IWindowActivated,
        IWindowDeactivated,
        IPositionUpdated,
        IIsEnabledChanged,
        ISizeUpdated,
        IFocusChange,
        IControl
    {
        IButton OpenSettingsButton { get; }

        IProjectListView ProjectList { get; }

        IMainViewStatusBar StatusBar { get; }
    }
}
