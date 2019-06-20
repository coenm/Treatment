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
    public class KeyUpRequestHandler : IRequestHandler
    {
        public bool CanHandle(IControlRequest request)
        {
            return request is KeyUpRequest;
        }

        public Task<IControlResponse> ExecuteAsync(IControlRequest request)
        {
            return ExecuteAsync(request as KeyUpRequest);
        }

        private Task<IControlResponse> ExecuteAsync(KeyUpRequest request)
        {
            Guard.NotNull(request, nameof(request));

            var keycodes = KeyCodesMapper.Map(request.KeyCodes);

            KeyboardInputGenerator.KeyUp(keycodes);

            return Task.FromResult(new KeyUpResponse() as IControlResponse);
        }
    }
}
