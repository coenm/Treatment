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
    public class KeyCombinationPressRequestHandler : IRequestHandler
    {
        public bool CanHandle(IRequest request)
        {
            return request is KeyCombinationPressRequest;
        }

        public Task<IResponse> ExecuteAsync(IRequest request)
        {
            return ExecuteAsync(request as KeyCombinationPressRequest);
        }

        private Task<IResponse> ExecuteAsync(KeyCombinationPressRequest request)
        {
            Guard.NotNull(request, nameof(request));

            var keycodes = KeyCodesMapper.Map(request.KeyCodes);

            KeyboardInputGenerator.KeyCombinationPress(keycodes);

            return Task.FromResult(new OkResponse() as IResponse);
        }
    }
}
