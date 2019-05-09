namespace TestAgent.RequestHandlers.Control
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Contract.Interface;
    using Contract.Interface.Control;
    using JetBrains.Annotations;
    using TestAgent.Implementation;
    using Treatment.Helpers.Guards;

    [UsedImplicitly]
    public class GetFileRequestHandler : IRequestHandler
    {
        public bool CanHandle(IControlRequest request) => request is GetFileRequest;

        public Task<IControlResponse> ExecuteAsync(IControlRequest request) => ExecuteAsync(request as GetFileRequest);

        private Task<IControlResponse> ExecuteAsync(GetFileRequest request)
        {
            Guard.NotNull(request, nameof(request));

            if (string.IsNullOrWhiteSpace(request.Filename))
                throw new ArgumentNullException(nameof(request.Filename));


            IControlResponse response = null;

            try
            {
                var data = File.ReadAllBytes(request.Filename);
                response = new GetFileResponse
                           {
                               Data = data
                           };
            }
            catch (Exception e)
            {
                response = new ExceptionResponse
                           {
                               Message = "Something went wrong reading the requested file. " + e.Message
                           };
            }

            return Task.FromResult(response);
        }
    }
}
