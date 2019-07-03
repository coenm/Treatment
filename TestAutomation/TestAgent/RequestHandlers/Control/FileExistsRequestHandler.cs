namespace TestAgent.RequestHandlers.Control
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using TestAgent.Contract.Interface;
    using TestAgent.Contract.Interface.Control;
    using TestAgent.Implementation;
    using Treatment.Helpers.Guards;

    [UsedImplicitly]
    public class FileExistsRequestHandler : IRequestHandler
    {
        [NotNull] private readonly IAgentContext context;

        public FileExistsRequestHandler([NotNull] IAgentContext context)
        {
            Guard.NotNull(context, nameof(context));
            this.context = context;
        }

        public bool CanHandle(IControlRequest request) => request is FileExistsRequest;

        public Task<IControlResponse> ExecuteAsync(IControlRequest request) => ExecuteAsync(request as FileExistsRequest);

        private Task<IControlResponse> ExecuteAsync(FileExistsRequest request)
        {
            Guard.NotNull(request, nameof(request));

            if (string.IsNullOrWhiteSpace(request.Filename))
                throw new ArgumentNullException(nameof(request.Filename));

            IControlResponse response = null;

            var contextWorkingDirectory = context.WorkingDirectory;

            if (string.IsNullOrWhiteSpace(contextWorkingDirectory))
            {
                response = new ExceptionResponse
                {
                    Message = "Working directory not set",
                };
                return Task.FromResult(response);
            }

            var filename = Path.Combine(contextWorkingDirectory, request.Filename);

            response = new FileExistsResponse
                {
                    FileExists = File.Exists(filename),
                };

            return Task.FromResult(response);
        }
    }
}
