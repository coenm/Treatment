namespace TestAgent
{
    using System;
    using System.Threading;

    using NLog;
    using NLog.Config;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using TestAgent.ZeroMq.PublishInfrastructure;
    using TestAgent.ZeroMq.RequestReplyInfrastructure;
    using TreatmentZeroMq.ContextService;
    using TreatmentZeroMq.Worker;
    using View;
    using ViewModel;
    using ZeroMQ;

    public static class Program
    {
#if DEBUG
        private const string Config = "Debug";
#else
        private const string Config = "Release";
#endif

        private static Container container;

        public static void Main(string[] args)
        {
            container = new Container();

            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.Options.AllowOverridingRegistrations = true;

            Bootstrapper.Bootstrap(
                container,
                $"tcp://*:{FixedSettings.AgentReqRspPort}",
                $"tcp://*:{FixedSettings.AgentPublishPort}",
                FixedSettings.SutPublishPort,
                FixedSettings.SutReqRspPort);

            // Views
            container.Register<MainWindow>();

            // View models
            container.Register<IMainWindowViewModel, MainWindowViewModel>(Lifestyle.Scoped);

            container.Verify(VerificationOption.VerifyOnly);

            var context = container.GetInstance<IZeroMqContextService>().GetContext();

            var cts = new CancellationTokenSource();

            var zeroMqReqRepProxyFactory = container.GetInstance<IZeroMqReqRepProxyFactory>();
            var reqRspProxy = zeroMqReqRepProxyFactory.Create();
            reqRspProxy.Start();

            var zeroMqPublishProxyFactory = container.GetInstance<IZeroMqPublishProxyFactory>();
            var publishProxy = zeroMqPublishProxyFactory.Create();
            publishProxy.Start();

            var workerManager = container.GetInstance<ReqRepWorkerManagement>();

            var workerTask = workerManager.StartSingleWorker(
                                container.GetInstance<IZeroMqRequestDispatcher>(),
                                "inproc://reqrsp",
                                cts.Token);

            // let listeners know agent has started.
            using (var agentPublishSocket = new ZSocket(context, ZSocketType.PUB))
            {
                agentPublishSocket.Connect("inproc://publish");
                agentPublishSocket.Send(new ZMessage(new[]
                {
                    new ZFrame("AGENT"),
                    new ZFrame("Started"),
                }));
            }

            Console.WriteLine("Done. Press enter to exit.");
//            Console.ReadKey();

            var agent = container.GetInstance<IAgentContext>();
            agent.Stop();

            Thread.Sleep(5000);

            publishProxy.Dispose();
            reqRspProxy.Dispose();
        }
    }
}
