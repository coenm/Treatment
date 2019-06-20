namespace TestAgent.RequestHandlers.Input.Keyboard
{
    using System.Threading.Tasks;

    using Dapplo.Windows.Input.Keyboard;
    using JetBrains.Annotations;
    using TestAgent.Contract.Interface;
    using TestAgent.Contract.Interface.Input.Keyboard;
    using TestAgent.Implementation;
    using TestAgent.RequestHandlers.Input.Keyboard.Mapper;
    using Treatment.Helpers.Guards;

    [UsedImplicitly]
    public class KeyCombinationPressRequestHandler : IRequestHandler
    {
        public bool CanHandle(IControlRequest request) => request is KeyCombinationPressRequest;

        public Task<IControlResponse> ExecuteAsync(IControlRequest request) => ExecuteAsync(request as KeyCombinationPressRequest);

        private Task<IControlResponse> ExecuteAsync(KeyCombinationPressRequest request)
        {
            Guard.NotNull(request, nameof(request));

            var keycodes = KeyCodesMapper.Map(request.KeyCodes);

            KeyboardInputGenerator.KeyCombinationPress(keycodes);

            return Task.FromResult(new KeyCombinationPressResponse() as IControlResponse);
        }
    }
}
