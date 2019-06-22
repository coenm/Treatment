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
    public class KeyPressesRequestHandler : IRequestHandler
    {
        public bool CanHandle(IControlRequest request)
        {
            return request is KeyPressesRequest;
        }

        public Task<IControlResponse> ExecuteAsync(IControlRequest request)
        {
            return ExecuteAsync(request as KeyPressesRequest);
        }

        private Task<IControlResponse> ExecuteAsync(KeyPressesRequest request)
        {
            Guard.NotNull(request, nameof(request));

            var keycodes = KeyCodesMapper.Map(request.KeyCodes);

            KeyboardInputGenerator.KeyPresses(keycodes);

            return Task.FromResult(new KeyPressesResponse() as IControlResponse);
        }
    }
}
