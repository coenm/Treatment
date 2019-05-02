namespace TestAgent.RequestHandlers.Input.Mouse
{
    using System.Threading.Tasks;
    using Contract.Interface;
    using Contract.Interface.Input.Mouse;
    using JetBrains.Annotations;
    using TestAgent.Implementation;
    using Treatment.Helpers.Guards;

    [PublicAPI]
    public class SingleClickRequestHandler : IRequestHandler
    {
        public bool CanHandle(IRequest request) => request is SingleClickRequest;

        public Task<IResponse> ExecuteAsync(IRequest request) => ExecuteAsync(request as SingleClickRequest);

        private Task<IResponse> ExecuteAsync(SingleClickRequest request)
        {
            Guard.NotNull(request, nameof(request));

            return Task.FromResult(new OkResponse() as IResponse);
        }
    }
}
