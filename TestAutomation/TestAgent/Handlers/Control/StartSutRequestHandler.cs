namespace TestAgent.Handlers.Control
{
    using System;
    using System.Threading.Tasks;
    using Implementation;
    using Interface;
    using Interface.Control;
    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;

    public class StartSutRequestHandler : IRequestHandler
    {
        private readonly SutContext context;

        public StartSutRequestHandler([NotNull] SutContext context)
        {
            Guard.NotNull(context, nameof(context));
            this.context = context;
        }

        public Task<IResponse> ExecuteAsync(IRequest request)
        {
            if (!(request is StartSutRequest r))
                throw new ArgumentException();

            // context.Start(r.Executable);
            return Task.FromResult(new StartSutResponse() as IResponse);

        }

        public bool CanHandle(IRequest request)
        {
            return request is StartSutRequest;
        }
    }
}
