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
    public class SingleClickRequestHandler : IRequestHandler
    {
        public bool CanHandle(IInputRequest request) => request is SingleClickRequest;

        public Task<IInputResponse> ExecuteAsync(IInputRequest request) => ExecuteAsync(request as SingleClickRequest);

        private async Task<IInputResponse> ExecuteAsync(SingleClickRequest request)
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
