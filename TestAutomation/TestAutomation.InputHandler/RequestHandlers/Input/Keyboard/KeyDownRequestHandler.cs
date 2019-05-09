namespace TestAutomation.InputHandler.RequestHandlers.Input.Keyboard
{
    using System.Threading.Tasks;

    using Dapplo.Windows.Input.Keyboard;
    using JetBrains.Annotations;
    using TestAutomation.InputHandler.RequestHandlers.Input.Keyboard.Mapper;
    using TestAutomation.Input.Contract.Interface;
    using TestAutomation.Input.Contract.Interface.Input.Keyboard;
    using Treatment.Helpers.Guards;

    [UsedImplicitly]
    public class KeyDownRequestHandler : IRequestHandler
    {
        public bool CanHandle(IInputRequest request)
        {
            return request is KeyDownRequest;
        }

        public Task<IInputResponse> ExecuteAsync(IInputRequest request)
        {
            return ExecuteAsync(request as KeyDownRequest);
        }

        private Task<IInputResponse> ExecuteAsync(KeyDownRequest request)
        {
            Guard.NotNull(request, nameof(request));

            var keycodes = KeyCodesMapper.Map(request.KeyCodes);

            KeyboardInputGenerator.KeyDown(keycodes);

            return Task.FromResult(new OkInputResponse() as IInputResponse);
        }
    }
}
