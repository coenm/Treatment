namespace Treatment.TestAutomation.TestRunner.Framework.RemoteImplementations
{
    using System;
    using System.Reactive.Disposables;
    using System.Threading.Tasks;
    using Contract.Interfaces.Events.Application;
    using global::TestAutomation.Input.Contract.Interface.Input.Enums;
    using global::TestAutomation.Input.Contract.Interface.Input.Keyboard;
    using global::TestAutomation.Input.Contract.Interface.Input.Mouse;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;

    internal class RemoteKeyboard : IKeyboard
    {
        private readonly IExecuteInput execute;

        public RemoteKeyboard(IExecuteInput execute)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public async Task<bool> PressAsync(params VirtualKeyCode[] keys)
        {
            var req = new KeyPressesRequest
            {
                KeyCodes = keys,
            };

            var result = await execute.ExecuteInput(req);

            return result is KeyPressesResponse;
        }

        public void Dispose()
        {
        }
    }
}
