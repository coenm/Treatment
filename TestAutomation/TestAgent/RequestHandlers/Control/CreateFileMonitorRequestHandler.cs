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
    public class CreateFileMonitorRequestHandler : IRequestHandler
    {
        [NotNull] private readonly IAgentContext context;

        public CreateFileMonitorRequestHandler([NotNull] IAgentContext context)
        {
            Guard.NotNull(context, nameof(context));
            this.context = context;
        }

        public bool CanHandle(IControlRequest request) => request is CreateFileMonitorRequest;

        public Task<IControlResponse> ExecuteAsync(IControlRequest request) => ExecuteAsync(request as CreateFileMonitorRequest);

        private Task<IControlResponse> ExecuteAsync(CreateFileMonitorRequest request)
        {
            Guard.NotNull(request, nameof(request));

            if (string.IsNullOrWhiteSpace(request.Filename))
                throw new ArgumentNullException(nameof(request.Filename));

            IControlResponse response = null;

            //todo

            return Task.FromResult(response);
        }
    }
}
