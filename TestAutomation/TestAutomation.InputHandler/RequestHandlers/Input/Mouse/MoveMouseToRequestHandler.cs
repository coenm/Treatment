﻿namespace TestAutomation.InputHandler.RequestHandlers.Input.Mouse
{
    using System.Threading.Tasks;

    using AxMouseManipulator;
    using JetBrains.Annotations;
    using TestAutomation.Input.Contract.Interface;
    using TestAutomation.Input.Contract.Interface.Input.Mouse;
    using Treatment.Helpers.Guards;

    [UsedImplicitly]
    public class MoveMouseToRequestHandler : IRequestHandler
    {
        public bool CanHandle(IInputRequest request) => request is MoveMouseToRequest;

        public Task<IInputResponse> ExecuteAsync(IInputRequest request) => ExecuteAsync(request as MoveMouseToRequest);

        private Task<IInputResponse> ExecuteAsync(MoveMouseToRequest request)
        {
            Guard.NotNull(request, nameof(request));

            MouseManipulator.SetCursorPosition(request.Position.X, request.Position.Y);
            return Task.FromResult(new MoveMouseToResponse() as IInputResponse);
        }
    }
}
