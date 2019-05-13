namespace Treatment.TestAutomation.TestRunner.Framework.RemoteImplementations
{
    using System;
    using System.Reactive.Disposables;
    using Contract.Interfaces.Events.Application;
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
