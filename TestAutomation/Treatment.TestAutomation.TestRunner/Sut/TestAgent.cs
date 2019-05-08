namespace Treatment.TestAutomation.TestRunner.Sut
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using global::TestAgent.Contract.Interface.Control;
    using TreatmentZeroMq.Helpers;
    using TreatmentZeroMq.Socket;
    using ZeroMQ;

    internal class TestAgent : ITestAgent
    {
        private readonly ZSocket socket;
        private readonly IExecuteControl execute;

        public TestAgent(IZeroMqSocketFactory socketFactory, string endpoint)
        {
            socket = socketFactory.Create(ZSocketType.REQ);
            socket.TryConnect(endpoint);
            ZmqConnection.GiveZeroMqTimeToFinishConnectOrBind();
            execute = new MyExecuteControl(socket);
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
            var req = new StartSutRequest();

            var rsp = await execute.ExecuteControl(req);

            if (rsp is StartSutResponse x)
                return x.Success;

            throw new Exception("something went wrong ;-)");
        }

        public void Dispose()
        {
            socket?.Dispose();
        }
    }
}
