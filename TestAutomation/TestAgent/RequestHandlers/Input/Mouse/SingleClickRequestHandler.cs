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
    public class SingleClickRequestHandler : IRequestHandler
    {
        public bool CanHandle(IControlRequest request) => request is SingleClickRequest;

        public Task<IControlResponse> ExecuteAsync(IControlRequest request) => ExecuteAsync(request as SingleClickRequest);

        private async Task<IControlResponse> ExecuteAsync(SingleClickRequest request)
        {
            Guard.NotNull(request, nameof(request));

            // MouseInputGenerator.MouseClick(MouseButtons.Left);
            MouseInputGenerator.MouseDown(MouseButtons.Left);

            await Task.Delay(10);

            MouseInputGenerator.MouseUp(MouseButtons.Left);

            return new SingleClickResponse();
        }
    }
}
