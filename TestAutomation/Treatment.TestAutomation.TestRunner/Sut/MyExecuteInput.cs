namespace Treatment.TestAutomation.TestRunner.Sut
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::TestAutomation.Input.Contract.Interface;
    using global::TestAutomation.Input.Contract.Serializer;
    using JetBrains.Annotations;
    using TreatmentZeroMq.Helpers;
    using ZeroMQ;

    public class MyExecuteInput : IExecuteInput
    {
        private readonly ZSocket socket;

        public MyExecuteInput([NotNull] ZSocket socket)
        {
            this.socket = socket;
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

            if (!socket.TrySend(msg))
                throw new Exception("something went wrong ;-)");

            if (!socket.TryReceive(out var rsp, 5, i => i * 10))
                throw new Exception("something went wrong ;-)");

            if (rsp.Count >= 2)
            {
                var t = rsp.Pop().ReadString();
                var p = rsp.Pop().ReadString();
                var resp = InputRequestResponseSerializer.DeserializeResponse(t, p);
                return Task.FromResult(resp);
            }

            throw new Exception("something went wrong ;-)");
        }
    }
}