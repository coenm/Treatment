namespace TestAutomation.InputHandler.RequestHandlers.Input.Mouse
{
    using System.Threading.Tasks;
    using Contract.Input.Interface;
    using Contract.Input.Interface.Input.Mouse;
    using Dapplo.Windows.Common.Structs;
    using Dapplo.Windows.Input.Mouse;
    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using MouseButtons = Dapplo.Windows.Input.Enums.MouseButtons;

    [PublicAPI]
    public class SingleClickRequestHandler : IRequestHandler
    {
        public bool CanHandle(IRequest request) => request is SingleClickRequest;

        public Task<IResponse> ExecuteAsync(IRequest request) => ExecuteAsync(request as SingleClickRequest);

        private async Task<IResponse> ExecuteAsync(SingleClickRequest request)
        {
            Guard.NotNull(request, nameof(request));

            MouseInputGenerator.MoveMouse(new NativePoint(0, 0));
            await Task.Delay(1000);
            // MouseInputGenerator.MouseDown(MouseButtons.Left, new NativePoint(0, 0));

            await Task.Delay(5000);

            MouseInputGenerator.MoveMouse(new NativePoint(1000, 1000));
            await Task.Delay(1000);
            // await Task.Delay(5000);

            // MouseInputGenerator.MouseUp(MouseButtons.Left, new NativePoint(1000, 1000));
            await Task.Delay(5000);

            MouseInputGenerator.MoveMouse(new NativePoint(1900, 1100));
            await Task.Delay(1000);

            MouseInputGenerator.MouseDown(MouseButtons.Left, new NativePoint(1900, 1100));
            await Task.Delay(1000);

            MouseInputGenerator.MouseUp(MouseButtons.Left, new NativePoint(1900, 1100));
            return new OkResponse {Msg = "Check8"};
        }
    }
}
