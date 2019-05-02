namespace TestAgent.RequestHandlers.Input.Mouse
{
    using System.Threading.Tasks;
    using Contract.Interface;
    using Contract.Interface.Input.Mouse;
    using JetBrains.Annotations;

    using TestAgent.Implementation;
    using Treatment.Helpers.Guards;

    [PublicAPI]
    public class DoubleClickRequestHandler : IRequestHandler
    {
        public bool CanHandle(IRequest request) => request is DoubleClickRequest;

        public Task<IResponse> ExecuteAsync(IRequest request) => ExecuteAsync(request as DoubleClickRequest);

        private Task<IResponse> ExecuteAsync(DoubleClickRequest request)
        {
            Guard.NotNull(request, nameof(request));

            return Task.FromResult(new OkResponse() as IResponse);
        }
    }
}
