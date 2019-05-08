namespace TestAutomation.InputHandler.RequestHandlers.Input.Mouse
{
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using TestAutomation.Input.Contract.Interface;
    using TestAutomation.Input.Contract.Interface.Input.Mouse;
    using Treatment.Helpers.Guards;

    [PublicAPI]
    public class DoubleClickRequestHandler : IRequestHandler
    {
        public bool CanHandle(IInputRequest request) => request is DoubleClickRequest;

        public Task<IInputResponse> ExecuteAsync(IInputRequest request) => ExecuteAsync(request as DoubleClickRequest);

        private Task<IInputResponse> ExecuteAsync(DoubleClickRequest request)
        {
            Guard.NotNull(request, nameof(request));

            return Task.FromResult(new OkInputResponse() as IInputResponse);
        }
    }
}
