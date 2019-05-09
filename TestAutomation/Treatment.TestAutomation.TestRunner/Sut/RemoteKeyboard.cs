namespace Treatment.TestAutomation.TestRunner.Sut
{
    using System;

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
