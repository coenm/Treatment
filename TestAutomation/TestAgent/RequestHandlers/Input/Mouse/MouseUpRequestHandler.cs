namespace TestAgent.RequestHandlers.Input.Mouse
{
    using System.Threading.Tasks;

    using Dapplo.Windows.Input.Mouse;
    using JetBrains.Annotations;
    using TestAgent.Contract.Interface;
    using TestAgent.Contract.Interface.Input.Mouse;
    using TestAgent.Implementation;
    using Treatment.Helpers.Guards;

    using MouseButtons = Dapplo.Windows.Input.Enums.MouseButtons;

    [UsedImplicitly]
    public class MouseUpRequestHandler : IRequestHandler
    {
        public bool CanHandle(IControlRequest request) => request is MouseUpRequest;

        public Task<IControlResponse> ExecuteAsync(IControlRequest request) => ExecuteAsync(request as MouseUpRequest);

        private Task<IControlResponse> ExecuteAsync(MouseUpRequest request)
        {
            Guard.NotNull(request, nameof(request));

            MouseInputGenerator.MouseUp(MouseButtons.Left);

            return Task.FromResult(new SingleClickResponse() as IControlResponse);
        }
    }
}
