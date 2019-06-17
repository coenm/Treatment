namespace Treatment.TestAutomation.TestRunner.Framework.RemoteImplementations
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using TestAgent.Contract.Interface.Control;

    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;

    internal class RemoteTestAgent : ITestAgent
    {
        private readonly IExecuteControl execute;
        private readonly ISutSettings sutSettings;

        public RemoteTestAgent(
            [NotNull] IExecuteControl execute,
            [NotNull] ISutSettings sutSettings)
        {
            Guard.NotNull(execute, nameof(execute));
            Guard.NotNull(sutSettings, nameof(sutSettings));

            this.execute = execute;
            this.sutSettings = sutSettings;
        }

        public async Task<List<string>> LocateFilesAsync(string directory, string filename)
        {
            var req = new LocateFilesRequest
            {
                Directory = directory,
                Filename = filename,
            };

            var rsp = await execute.ExecuteControl(req);

            if (rsp is LocateFilesResponse x)
                return x.Executable;

            throw new Exception("something went wrong ;-)");
        }

        public async Task<bool> StartSutAsync()
        {
            var req = new StartSutRequest
                      {
                          Executable = sutSettings.SutExecutable,
                          WorkingDirectory = sutSettings.WorkingDirectory,
                      };

            var rsp = await execute.ExecuteControl(req);

            if (rsp is StartSutResponse x)
                return x.Success;

            throw new Exception("something went wrong ;-)");
        }

        public async Task<byte[]> GetFileContentAsync(string filename)
        {
            var req = new GetFileRequest
                      {
                          Filename = filename,
                      };

            var rsp = await execute.ExecuteControl(req);

            if (rsp is GetFileResponse x)
                return x.Data;

            throw new Exception($"something went wrong. Received: {rsp.GetType().FullName}");
        }

        public async Task<string> LocateSutExecutableAsync()
        {
            var req = new GetSutExecutableRequest();

            var rsp = await execute.ExecuteControl(req);

            if (rsp is GetSutExecutableResponse x)
                return x.Filename;

            throw new Exception("something went wrong ;-)");
        }

        public void Dispose()
        {
        }
    }
}
