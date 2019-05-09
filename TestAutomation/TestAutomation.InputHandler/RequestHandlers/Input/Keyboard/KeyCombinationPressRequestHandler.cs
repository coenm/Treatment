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
    public class KeyCombinationPressRequestHandler : IRequestHandler
    {
        public bool CanHandle(IInputRequest request) => request is KeyCombinationPressRequest;

        public Task<IInputResponse> ExecuteAsync(IInputRequest request) => ExecuteAsync(request as KeyCombinationPressRequest);

        private Task<IInputResponse> ExecuteAsync(KeyCombinationPressRequest request)
        {
            Guard.NotNull(request, nameof(request));

            var keycodes = KeyCodesMapper.Map(request.KeyCodes);

            KeyboardInputGenerator.KeyCombinationPress(keycodes);

            return Task.FromResult(new OkInputResponse() as IInputResponse);
        }
    }
}
