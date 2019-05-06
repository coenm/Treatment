namespace TestAutomation.InputHandler.RequestHandlers.Input.Keyboard
{
    using System.Threading.Tasks;
    using Contract.Input.Interface;
    using Contract.Input.Interface.Input.Keyboard;
    using Dapplo.Windows.Input.Keyboard;
    using JetBrains.Annotations;
    using Mapper;
    using Treatment.Helpers.Guards;

    [UsedImplicitly]
    public class KeyPressesRequestHandler : IRequestHandler
    {
        public bool CanHandle(IRequest request)
        {
            return request is KeyPressesRequest;
        }

        public Task<IResponse> ExecuteAsync(IRequest request)
        {
            return ExecuteAsync(request as KeyPressesRequest);
        }

        private Task<IResponse> ExecuteAsync(KeyPressesRequest request)
        {
            Guard.NotNull(request, nameof(request));

            var keycodes = KeyCodesMapper.Map(request.KeyCodes);

            KeyboardInputGenerator.KeyPresses(keycodes);

            return Task.FromResult(new OkResponse() as IResponse);
        }
    }
}
