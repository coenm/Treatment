namespace Treatment.TestAutomation.TestRunner.Framework.RemoteImplementations
{
    using System;

    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;

    internal class RemoteKeyboard : IKeyboard
    {
        private readonly IExecuteInput execute;

        public RemoteKeyboard(IExecuteInput execute)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public void Dispose()
        {
        }
    }
}
