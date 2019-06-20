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
    public class DoubleClickRequestHandler : IRequestHandler
    {
        public bool CanHandle(IControlRequest request) => request is DoubleClickRequest;

        public Task<IControlResponse> ExecuteAsync(IControlRequest request) => ExecuteAsync(request as DoubleClickRequest);

        private async Task<IControlResponse> ExecuteAsync(DoubleClickRequest request)
        {
            Guard.NotNull(request, nameof(request));

            MouseInputGenerator.MouseClick(MouseButtons.Left);

            await Task.Delay(100);

            MouseInputGenerator.MouseClick(MouseButtons.Left);

            return new DoubleClickResponse();
        }
    }
}
