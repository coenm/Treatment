namespace TestAgent.ViewModel
{
    using System;
    using System.Threading;

    using JetBrains.Annotations;
    using TestAgent.ZeroMq.PublishInfrastructure;
    using TestAgent.ZeroMq.RequestReplyInfrastructure;
    using TreatmentZeroMq.ContextService;
    using TreatmentZeroMq.Worker;
    using Wpf.Framework.ViewModel;
    using ZeroMQ;

    public class TestAgentMainWindowViewModel : ViewModelBase, ITestAgentMainWindowViewModel, IDisposable
    {
        [NotNull] private readonly IAgentContext agent;
        [NotNull] private readonly CancellationTokenSource cts;
        [NotNull] private readonly ZeroMqReqRepProxyService reqRspProxy;
        [NotNull] private readonly ZeroMqPublishProxyService publishProxy;

        public TestAgentMainWindowViewModel(
            [NotNull] IZeroMqContextService contextService,
            [NotNull] IZeroMqReqRepProxyFactory zeroMqReqRepProxyFactory,
            [NotNull] IZeroMqPublishProxyFactory zeroMqPublishProxyFactory,
            [NotNull] ReqRepWorkerManagement workerManager,
            [NotNull] IZeroMqRequestDispatcher zmqDispatcher,
            [NotNull] IAgentContext agent)
        {
            this.agent = agent;
            var context = contextService.GetContext();

            cts = new CancellationTokenSource();

            reqRspProxy = zeroMqReqRepProxyFactory.Create();
            reqRspProxy.Start();

            publishProxy = zeroMqPublishProxyFactory.Create();
            publishProxy.Start();

            var workerTask = workerManager.StartSingleWorker(
                zmqDispatcher,
                "inproc://reqrsp",
                cts.Token);

            // let listeners know agent has started.
            using (var agentPublishSocket = new ZSocket(context, ZSocketType.PUB))
            {
                agentPublishSocket.Connect("inproc://publish");
                agentPublishSocket.Send(new ZMessage(new[] { new ZFrame("AGENT"), new ZFrame("Started"), }));
            }
        }

        public void Dispose()
        {
            cts.Cancel();
            agent.Stop();
            publishProxy.Dispose();
            reqRspProxy.Dispose();
        }
    }
}
