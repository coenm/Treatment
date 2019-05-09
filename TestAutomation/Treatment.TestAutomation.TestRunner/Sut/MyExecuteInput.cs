namespace Treatment.TestAutomation.TestRunner.Sut
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using global::TestAutomation.Input.Contract.Interface;
    using global::TestAutomation.Input.Contract.Serializer;
    using JetBrains.Annotations;
    using TreatmentZeroMq.Helpers;
    using TreatmentZeroMq.Socket;
    using ZeroMQ;

    public class MyExecuteInput : IExecuteInput
    {
        [NotNull] private readonly IZeroMqSocketFactory socketFactory;
        [NotNull] private readonly string endpoint;

        public MyExecuteInput([NotNull] IZeroMqSocketFactory socketFactory, [NotNull] string endpoint)
        {
            this.socketFactory = socketFactory;
            this.endpoint = endpoint;
        }

        public Task<IInputResponse> ExecuteInput(IInputRequest request)
        {
            if (request == null)
                return null;

            var (type, payload) = InputRequestResponseSerializer.Serialize(request);

            var msg = new ZMessage(
               new List<ZFrame>
               {
                   new ZFrame("SUT"),
                   new ZFrame(type),
                   new ZFrame(payload),
               });

            using (var socket = socketFactory.Create(ZSocketType.REQ))
            {
                socket.TryConnect(endpoint);
                ZmqConnection.GiveZeroMqTimeToFinishConnectOrBind();

                if (!socket.TrySend(msg))
                    throw new Exception("something went wrong ;-)");

                if (!socket.TryReceive(out var rsp, 5, i => i * 10))
                    throw new Exception("something went wrong ;-)");

                if (rsp.Count < 2)
                    throw new Exception("something went wrong ;-)");

                var t = rsp.Pop().ReadString();
                var p = rsp.Pop().ReadString();
                var resp = InputRequestResponseSerializer.DeserializeResponse(t, p);
                return Task.FromResult(resp);
            }
        }
    }
}
