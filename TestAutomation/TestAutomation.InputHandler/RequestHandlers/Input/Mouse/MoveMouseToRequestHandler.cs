namespace TestAutomation.InputHandler.RequestHandlers.Input.Mouse
{
    using System.Threading.Tasks;
    using Contract.Input.Interface;
    using Contract.Input.Interface.Input.Mouse;
    using Dapplo.Windows.Common.Structs;
    using Dapplo.Windows.Input.Mouse;
    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;

    [PublicAPI]
    public class MoveMouseToRequestHandler : IRequestHandler
    {
        public bool CanHandle(IRequest request) => request is MoveMouseToRequest;

        public Task<IResponse> ExecuteAsync(IRequest request) => ExecuteAsync(request as MoveMouseToRequest);

        private Task<IResponse> ExecuteAsync(MoveMouseToRequest request)
        {
            Guard.NotNull(request, nameof(request));

            // var winProcHandler = WinProcHandler.Instance;
            MouseInputGenerator.MoveMouse(new NativePoint(request.Position.X, request.Position.Y));

            return Task.FromResult(new OkResponse() as IResponse);
        }
    }
}
