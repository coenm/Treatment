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
    public class MouseUpRequestHandler : IRequestHandler
    {
        public bool CanHandle(IInputRequest request) => request is MouseUpRequest;

        public Task<IInputResponse> ExecuteAsync(IInputRequest request) => ExecuteAsync(request as MouseUpRequest);

        private Task<IInputResponse> ExecuteAsync(MouseUpRequest request)
        {
            Guard.NotNull(request, nameof(request));

            MouseInputGenerator.MouseUp(MouseButtons.Left);

            return Task.FromResult(new SingleClickResponse() as IInputResponse);
        }
    }
}
