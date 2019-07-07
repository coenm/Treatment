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
    public class DeleteFileRequestHandler : IRequestHandler
    {
        [NotNull] private readonly IAgentContext context;

        public DeleteFileRequestHandler([NotNull] IAgentContext context)
        {
            Guard.NotNull(context, nameof(context));
            this.context = context;
        }

        public bool CanHandle(IControlRequest request) => request is DeleteFileRequest;

        public Task<IControlResponse> ExecuteAsync(IControlRequest request) => ExecuteAsync(request as DeleteFileRequest);

        private Task<IControlResponse> ExecuteAsync(DeleteFileRequest request)
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

            try
            {
                var filename = Path.Combine(contextWorkingDirectory, request.Filename);
                if (!File.Exists(filename))
                {
                    response = new DeleteFileResponse
                    {
                        Deleted = true,
                    };
                }
                else
                {
                    File.Delete(filename);
                    response = new DeleteFileResponse
                        {
                            Deleted = true,
                        };
                }
            }
            catch (Exception)
            {
                response = new DeleteFileResponse
                {
                    Deleted = false,
                };
            }

            return Task.FromResult(response);
        }
    }
}
