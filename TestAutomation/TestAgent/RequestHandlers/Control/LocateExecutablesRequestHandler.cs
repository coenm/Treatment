namespace TestAgent.RequestHandlers.Control
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using TestAgent.Implementation;
    using TestAgent.Interface;
    using TestAgent.Interface.Control;

    using Treatment.Helpers.Guards;

    [UsedImplicitly]
    public class LocateExecutablesRequestHandler : IRequestHandler
    {
        public bool CanHandle(IRequest request) => request is LocateExecutablesRequest;

        public Task<IResponse> ExecuteAsync(IRequest request) => ExecuteAsync(request as LocateExecutablesRequest);

        protected virtual IEnumerable<string> FindFilesIncludingSubdirectories(string rootPath, string mask)
        {
            // note this this might take a while.
            // can throw exceptions..
            return Directory.GetFiles(rootPath, mask, SearchOption.AllDirectories);
        }

        private Task<IResponse> ExecuteAsync(LocateExecutablesRequest request)
        {
            Guard.NotNull(request, nameof(request));

            if (string.IsNullOrWhiteSpace(request.Directory))
                throw new ArgumentNullException(nameof(request.Directory));

            if (string.IsNullOrWhiteSpace(request.Filename))
                throw new ArgumentNullException(nameof(request.Directory));

            var files = FindFilesIncludingSubdirectories(request.Directory, request.Filename);

            var response = new LocateExecutablesResponse
                {
                    Executable = files.ToList()
                };
            return Task.FromResult((IResponse)response);
        }
    }
}
