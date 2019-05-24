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

            // MouseInputGenerator.MoveMouse(new NativePoint(0, 0));
            // await Task.Delay(1000);
            // // MouseInputGenerator.MouseDown(MouseButtons.Left, new NativePoint(0, 0));
            //
            // await Task.Delay(5000);
            //
            // MouseInputGenerator.MoveMouse(new NativePoint(1000, 1000));
            // await Task.Delay(1000);
            // // await Task.Delay(5000);
            //
            // // MouseInputGenerator.MouseUp(MouseButtons.Left, new NativePoint(1000, 1000));
            // await Task.Delay(5000);
            //
            // MouseInputGenerator.MoveMouse(new NativePoint(1900, 1100));
            // await Task.Delay(1000);
            //
            // MouseInputGenerator.MouseDown(MouseButtons.Left, new NativePoint(1900, 1100));
            // await Task.Delay(1000);
            //
            // MouseInputGenerator.MouseUp(MouseButtons.Left, new NativePoint(1900, 1100));

            await Task.CompletedTask;
            return new SingleClickResponse();
        }
    }
}
