namespace Treatment.TestAutomation.TestRunner.Sut
{
    using System;

    public class ZeroMqRemoteKeyboard : IKeyboard
    {
        private readonly IExecuteInput execute;

        public ZeroMqRemoteKeyboard(IExecuteInput execute)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public void Dispose()
        {
        }
    }
}
