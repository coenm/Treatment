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
    public class KeyUpRequestHandler : IRequestHandler
    {
        public bool CanHandle(IInputRequest request)
        {
            return request is KeyUpRequest;
        }

        public Task<IInputResponse> ExecuteAsync(IInputRequest request)
        {
            return ExecuteAsync(request as KeyUpRequest);
        }

        private Task<IInputResponse> ExecuteAsync(KeyUpRequest request)
        {
            Guard.NotNull(request, nameof(request));

            var keycodes = KeyCodesMapper.Map(request.KeyCodes);

            KeyboardInputGenerator.KeyUp(keycodes);

            return Task.FromResult(new KeyUpResponse() as IInputResponse);
        }
    }
}
