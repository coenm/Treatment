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
        [NotNull] private readonly ISutExecutable sutExecutable;

        public GetSutExecutableRequestHandler([NotNull] ISutExecutable sutExecutable)
        {
            Guard.NotNull(sutExecutable, nameof(sutExecutable));

            this.sutExecutable = sutExecutable;
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
                               Filename = sutExecutable.Executable,
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
