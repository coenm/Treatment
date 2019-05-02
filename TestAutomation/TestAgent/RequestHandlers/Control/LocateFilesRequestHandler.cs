namespace TestAgent.RequestHandlers.Control
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Contract.Interface;
    using Contract.Interface.Control;
    using JetBrains.Annotations;
    using TestAgent.Implementation;
    using Treatment.Helpers.Guards;

    [UsedImplicitly]
    public class LocateFilesRequestHandler : IRequestHandler
    {
        public bool CanHandle(IRequest request) => request is LocateFilesRequest;

        public Task<IResponse> ExecuteAsync(IRequest request) => ExecuteAsync(request as LocateFilesRequest);

        protected virtual IEnumerable<string> FindFilesIncludingSubdirectories(string rootPath, string mask)
        {
            // note this this might take a while.
            // can throw exceptions..
            try
            {
                return Directory.GetFiles(rootPath, mask, SearchOption.AllDirectories);
            }
            catch (Exception e)
            {
                return new List<string>
                {
                    "This is stupid..",
                    e.Message,
                };
            }
        }

        private Task<IResponse> ExecuteAsync(LocateFilesRequest request)
        {
            Guard.NotNull(request, nameof(request));

            if (string.IsNullOrWhiteSpace(request.Directory))
                throw new ArgumentNullException(nameof(request.Directory));

            if (string.IsNullOrWhiteSpace(request.Filename))
                throw new ArgumentNullException(nameof(request.Directory));

            var files = FindFilesIncludingSubdirectories(request.Directory, request.Filename);

            var response = new LocateFilesResponse
                {
                    Executable = files.ToList()
                };
            return Task.FromResult((IResponse)response);
        }
    }
}
