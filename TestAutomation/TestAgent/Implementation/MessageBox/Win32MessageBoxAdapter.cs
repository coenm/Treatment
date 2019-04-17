namespace TestAgent.Implementation.MessageBox
{
    using System;
    using Interface;

    public class Win32MessageBoxAdapter : IMessageBox
    {
        public event EventHandler Closed;

        public Win32MessageBoxAdapter()
        {

        }

        public string Title { get; }

        public string Message { get; }

        public void Close()
        {
        }

        public MessageBoxButtons[] GetAvailableButtons()
        {
            return null;
        }

        public bool TryPressButton(MessageBoxButtons button)
        {
            return true;
        }
    }
}
