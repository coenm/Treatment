namespace TestAgent.RequestHandlers.Input.Keyboard
{
    using System.Threading.Tasks;

    using Dapplo.Windows.Input.Keyboard;

    using JetBrains.Annotations;

    using TestAgent.Implementation;
    using TestAgent.Interface;
    using TestAgent.Interface.Input.Keyboard;
    using TestAgent.RequestHandlers.Input.Keyboard.Mapper;

    using Treatment.Helpers.Guards;

    [UsedImplicitly]
    public class KeyUpRequestHandler : IRequestHandler
    {
        public bool CanHandle(IRequest request)
        {
            return request is KeyUpRequest;
        }

        public Task<IResponse> ExecuteAsync(IRequest request)
        {
            return ExecuteAsync(request as KeyUpRequest);
        }

        private Task<IResponse> ExecuteAsync(KeyUpRequest request)
        {
            Guard.NotNull(request, nameof(request));

            var keycodes = KeyCodesMapper.Map(request.KeyCodes);

            KeyboardInputGenerator.KeyUp(keycodes);

            return Task.FromResult(new OkResponse() as IResponse);
        }
    }
}
