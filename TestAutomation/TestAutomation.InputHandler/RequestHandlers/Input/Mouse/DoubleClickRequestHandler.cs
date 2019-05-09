namespace TestAutomation.InputHandler.RequestHandlers.Input.Mouse
{
    using System.Threading.Tasks;

    using Dapplo.Windows.Input.Mouse;
    using JetBrains.Annotations;
    using TestAutomation.Input.Contract.Interface;
    using TestAutomation.Input.Contract.Interface.Input.Mouse;
    using Treatment.Helpers.Guards;
    using MouseButtons = Dapplo.Windows.Input.Enums.MouseButtons;

    [UsedImplicitly]
    public class DoubleClickRequestHandler : IRequestHandler
    {
        public bool CanHandle(IInputRequest request) => request is DoubleClickRequest;

        public Task<IInputResponse> ExecuteAsync(IInputRequest request) => ExecuteAsync(request as DoubleClickRequest);

        private async Task<IInputResponse> ExecuteAsync(DoubleClickRequest request)
        {
            Guard.NotNull(request, nameof(request));

            MouseInputGenerator.MouseClick(MouseButtons.Left);

            await Task.Delay(100);

            MouseInputGenerator.MouseClick(MouseButtons.Left);

            return new DoubleClickResponse();
        }
    }
}
