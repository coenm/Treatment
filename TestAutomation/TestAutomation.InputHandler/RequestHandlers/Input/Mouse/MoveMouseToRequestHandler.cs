namespace TestAutomation.InputHandler.RequestHandlers.Input.Mouse
{
    using System.Threading.Tasks;

    using AxMouseManipulator;
    using Dapplo.Windows.Common.Structs;
    using Dapplo.Windows.Input.Mouse;
    using JetBrains.Annotations;
    using TestAutomation.Input.Contract.Interface;
    using TestAutomation.Input.Contract.Interface.Input.Mouse;
    using Treatment.Helpers.Guards;

    [PublicAPI]
    public class MoveMouseToRequestHandler : IRequestHandler
    {
        public bool CanHandle(IInputRequest request) => request is MoveMouseToRequest;

        public Task<IInputResponse> ExecuteAsync(IInputRequest request) => ExecuteAsync(request as MoveMouseToRequest);

        private Task<IInputResponse> ExecuteAsync(MoveMouseToRequest request)
        {
            Guard.NotNull(request, nameof(request));

            var pos = AxMouseManipulator.MouseManipulator.GetCursorPosition();

            MouseManipulator.SetCursorPosition(request.Position.X, request.Position.Y);

            // var winProcHandler = WinProcHandler.Instance;
            // MouseInputGenerator.MoveMouse(new NativePoint(request.Position.X, request.Position.Y));



            return Task.FromResult(new OkInputResponse
                                   {
                                       Msg = pos.X + "   " + pos.Y,
                                   }as IInputResponse);
        }
    }
}
