namespace TestAgent.RequestHandlers.Input.Mouse
{
    using System.Threading.Tasks;

    using AxMouseManipulator;
    using JetBrains.Annotations;
    using TestAgent.Contract.Interface;
    using TestAgent.Contract.Interface.Input.Mouse;
    using TestAgent.Implementation;
    using Treatment.Helpers.Guards;

    [UsedImplicitly]
    public class MoveMouseToRequestHandler : IRequestHandler
    {
        public bool CanHandle(IControlRequest request) => request is MoveMouseToRequest;

        public Task<IControlResponse> ExecuteAsync(IControlRequest request) => ExecuteAsync(request as MoveMouseToRequest);

        private Task<IControlResponse> ExecuteAsync(MoveMouseToRequest request)
        {
            Guard.NotNull(request, nameof(request));

            MouseManipulator.SetCursorPosition(request.Position.X, request.Position.Y);
            return Task.FromResult(new MoveMouseToResponse() as IControlResponse);
        }
    }
}
