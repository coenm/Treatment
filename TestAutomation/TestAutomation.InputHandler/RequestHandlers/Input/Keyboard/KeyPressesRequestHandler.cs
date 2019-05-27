namespace TestAutomation.InputHandler.RequestHandlers.Input.Keyboard
{
    using System.Threading.Tasks;

    using Dapplo.Windows.Input.Keyboard;
    using JetBrains.Annotations;
    using TestAutomation.Input.Contract.Interface;
    using TestAutomation.Input.Contract.Interface.Input.Keyboard;
    using TestAutomation.InputHandler.RequestHandlers.Input.Keyboard.Mapper;
    using Treatment.Helpers.Guards;

    [UsedImplicitly]
    public class KeyPressesRequestHandler : IRequestHandler
    {
        public bool CanHandle(IInputRequest request)
        {
            return request is KeyPressesRequest;
        }

        public Task<IInputResponse> ExecuteAsync(IInputRequest request)
        {
            return ExecuteAsync(request as KeyPressesRequest);
        }

        private Task<IInputResponse> ExecuteAsync(KeyPressesRequest request)
        {
            Guard.NotNull(request, nameof(request));

            var keycodes = KeyCodesMapper.Map(request.KeyCodes);

            KeyboardInputGenerator.KeyPresses(keycodes);

            return Task.FromResult(new KeyPressesResponse() as IInputResponse);
        }
    }
}
