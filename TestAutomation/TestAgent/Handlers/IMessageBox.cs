namespace TestAgent.Handlers
{
    using System;
    using JetBrains.Annotations;

    public interface IMessageBox
    {
        event EventHandler Closed;

        string Title { get; }

        string Message { get; }

        void Close();

        [NotNull]
        MessageBoxButtons[] GetAvailableButtons();

        bool TryPressButton(MessageBoxButtons button);
    }
}
