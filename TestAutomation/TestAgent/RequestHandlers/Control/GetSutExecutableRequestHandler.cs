namespace TestAgent.RequestHandlers.Control
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using TestAgent.Contract.Interface;
    using TestAgent.Contract.Interface.Control;
    using TestAgent.Implementation;
    using Treatment.Helpers.Guards;

    [UsedImplicitly]
    public class GetSutExecutableRequestHandler : IRequestHandler
    {
        [NotNull] private readonly IResolveSutExecutable resolveSutExecutable;

        public GetSutExecutableRequestHandler([NotNull] IResolveSutExecutable resolveSutExecutable)
        {
            Guard.NotNull(resolveSutExecutable, nameof(resolveSutExecutable));

            this.resolveSutExecutable = resolveSutExecutable;
        }

        public bool CanHandle(IControlRequest request) => request is GetSutExecutableRequest;

        public Task<IControlResponse> ExecuteAsync(IControlRequest request) => ExecuteAsync(request as GetSutExecutableRequest);

        private Task<IControlResponse> ExecuteAsync(GetSutExecutableRequest request)
        {
            Guard.NotNull(request, nameof(request));
            IControlResponse response;

            try
            {
                response = new GetSutExecutableResponse
                           {
                               Filename = resolveSutExecutable.Executable,
                           };
            }
            catch (Exception e)
            {
                response = new ExceptionResponse
                           {
                               Message = "Something went wrong reading the requested file. " + e.Message,
                           };
            }

            return Task.FromResult(response);
        }
    }
}
