namespace TestAgent.Handlers.Input.Keyboard
{
    using System.Threading.Tasks;
    using Dapplo.Windows.Input.Keyboard;
    using Implementation;
    using Interface;
    using Interface.Input.Keyboard;
    using JetBrains.Annotations;
    using Mapper;
    using Treatment.Helpers.Guards;

    [UsedImplicitly]
    public class KeyDownRequestHandler : IRequestHandler
    {
        public bool CanHandle(IRequest request)
        {
            return request is KeyDownRequest;
        }

        public Task<IResponse> ExecuteAsync(IRequest request)
        {
            return ExecuteAsync(request as KeyDownRequest);
        }

        private Task<IResponse> ExecuteAsync(KeyDownRequest request)
        {
            Guard.NotNull(request, nameof(request));

            var keycodes = KeyCodesMapper.Map(request.KeyCodes);

            KeyboardInputGenerator.KeyDown(keycodes);

            return Task.FromResult(new OkResponse() as IResponse);
        }
    }
}
