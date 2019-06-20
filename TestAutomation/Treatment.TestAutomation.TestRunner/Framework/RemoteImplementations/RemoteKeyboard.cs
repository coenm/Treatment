namespace Treatment.TestAutomation.TestRunner.Framework.RemoteImplementations
{
    using System;
    using System.Threading.Tasks;

    using TestAgent.Contract.Interface.Input.Enums;
    using TestAgent.Contract.Interface.Input.Keyboard;
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

        public async Task<bool> KeyDownAsync(params VirtualKeyCode[] keys)
        {
            var req = new KeyDownRequest
                      {
                          KeyCodes = keys,
                      };

            var result = await execute.ExecuteInput(req);

            return result is KeyDownResponse;
        }

        public async Task<bool> KeyUpAsync(params VirtualKeyCode[] keys)
        {
            var req = new KeyUpRequest
                      {
                          KeyCodes = keys,
                      };

            var result = await execute.ExecuteInput(req);

            return result is KeyUpResponse;
        }

        public async Task<bool> KeyCombinationPressAsync(params VirtualKeyCode[] keys)
        {
            var req = new KeyCombinationPressRequest
                      {
                          KeyCodes = keys,
                      };

            var result = await execute.ExecuteInput(req);

            return result is KeyCombinationPressResponse;
        }

        public void Dispose()
        {
        }
    }
}
