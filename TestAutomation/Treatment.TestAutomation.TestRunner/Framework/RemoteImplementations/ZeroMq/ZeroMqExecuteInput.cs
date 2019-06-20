namespace Treatment.TestAutomation.TestRunner.Framework.RemoteImplementations.ZeroMq
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CoenM.ZeroMq.Helpers;
    using CoenM.ZeroMq.Socket;
    using JetBrains.Annotations;
    using TestAgent.Contract.Interface;
    using TestAgent.Contract.Serializer;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;
    using ZeroMQ;

    [UsedImplicitly]
    internal class ZeroMqExecuteInput : IExecuteInput
    {
        [NotNull] private readonly IZeroMqSocketFactory socketFactory;
        [NotNull] private readonly string endpoint;

        public ZeroMqExecuteInput([NotNull] IZeroMqSocketFactory socketFactory, [NotNull] IAgentSettings agentSettings)
        {
            Guard.NotNull(socketFactory, nameof(socketFactory));
            Guard.NotNull(agentSettings, nameof(agentSettings));

            this.socketFactory = socketFactory;
            endpoint = agentSettings.ControlEndpoint;
        }

        public Task<IControlResponse> ExecuteInput(IControlRequest request)
        {
            if (request == null)
                return null;

            var (type, payload) = TestAgentRequestResponseSerializer.Serialize(request);

            var msg = new ZMessage(
               new List<ZFrame>
               {
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
                var resp = TestAgentRequestResponseSerializer.DeserializeResponse(t, p);
                return Task.FromResult(resp);
            }
        }
    }
}
