namespace TestAgent.RequestHandlers.Control
{
    using System.Threading.Tasks;
    using Contract.Interface;
    using Contract.Interface.Control;
    using JetBrains.Annotations;

    using TestAgent.Implementation;
    using Treatment.Helpers.Guards;

    public class StartSutRequestHandler : IRequestHandler
    {
        [NotNull] private readonly ISutContext context;

        public StartSutRequestHandler([NotNull] ISutContext context)
        {
            Guard.NotNull(context, nameof(context));
            this.context = context;
        }

        public bool CanHandle(IRequest request) => request is StartSutRequest;

        public Task<IResponse> ExecuteAsync(IRequest request) => ExecuteAsync(request as StartSutRequest);

        private Task<IResponse> ExecuteAsync(StartSutRequest request)
        {
            Guard.NotNull(request, nameof(request));

            return Task.FromResult(new OkResponse() as IResponse);
        }
    }
}
