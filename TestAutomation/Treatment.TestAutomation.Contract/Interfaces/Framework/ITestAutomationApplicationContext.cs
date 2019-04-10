namespace Treatment.TestAutomation.Contract.Interfaces.Framework
{
    using System;
    using JetBrains.Annotations;

    public interface ITestAutomationApplicationContext
    {
        event EventHandler MessageBoxShown;
        event EventHandler MessageBoxClosed;
        event EventHandler ApplicationStarted;
        event EventHandler ApplicationClosed;

        [CanBeNull]
        IMessageBox MessageBox { get; }

        [CanBeNull]
        ITestAutomationView MainView { get; }

        [NotNull]
        IMouse Mouse { get; }

        [NotNull]
        IKeyboard Keyboard { get; }
    }
}

