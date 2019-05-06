namespace TestAgent
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Medallion.Shell;
    using SimpleInjector;
    using TreatmentZeroMq.ContextService;
    using TreatmentZeroMq.Worker;
    using ZeroMq.PublishInfrastructure;
    using ZeroMq.RequestReplyInfrastructure;
    using ZeroMQ;

    public static class Program
    {
#if DEBUG
        private const string Config = "Debug";
#else
        private const string Config = "Release";
#endif

        private static Container container;

        public static async Task Main(string[] args)
        {
            container = new Container();

            Bootstrapper.Bootstrap(
                container,
                $"tcp://*:{Settings.AgentReqRspPort}",
                $"tcp://*:{Settings.AgentPublishPort}",
                Settings.SutPublishPort,
                Settings.SutReqRspPort);

            container.Verify(VerificationOption.VerifyOnly);

            var context = container.GetInstance<IZeroMqContextService>().GetContext();

            var mreListening = new ManualResetEvent(false);
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

            var task = Task.Run(() =>
            {
                int received = 0;
                using (var subscriber = new ZSocket(context, ZSocketType.SUB))
                using (cts.Token.Register(() => subscriber.Dispose()))
                {
                    subscriber.Connect($"tcp://localhost:{Settings.AgentPublishPort}");
                    subscriber.SubscribeAll();
                    mreListening.Set();

                    try
                    {
                        while (true)
                        {
                            var zmsg = new ZMessage();
                            ZError error;

                            if (!subscriber.ReceiveMessage(ref zmsg, ZSocketFlags.None, out error))
                            {
                                Console.WriteLine($" Oops, could not receive a request: {error}");
                                return;
                            }

                            received++;
                            Console.Title = $"Agent Received #{received}";

                            using (zmsg)
                            using (var zmsg2 = zmsg.Duplicate())
                            {
                                Console.WriteLine();
                                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:fff"));
                                Console.WriteLine("+" + new string('-', 100 + 2) + "+");

                                bool subscribeUnsubscribe = false;
                                if (zmsg2.Count == 1 && zmsg2[0].Length == 1)
                                {
                                    var b = zmsg2.PopAsByte();
                                    var m = string.Empty;
                                    if (b == 0x01)
                                        m = "SUBSCRIBE  (0x01)";
                                    if (b == 0x00)
                                        m = "UNSUBSCRIBE (0x00)";

                                    if (!string.IsNullOrEmpty(m))
                                    {
                                        subscribeUnsubscribe = true;
                                        Console.WriteLine($"| {m,-100} |");
                                    }
                                }

                                if (!subscribeUnsubscribe)
                                {
                                    foreach (var frame in zmsg)
                                    {
                                        var s = frame.ReadString();
                                        Console.WriteLine($"| {s,-100} |");
                                    }
                                }

                                Console.WriteLine("+" + new string('-', 100 + 2) + "+");
                                Console.WriteLine(" ");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (!cts.IsCancellationRequested)
                            Console.WriteLine(e.Message);
                    }
                }
            }).ConfigureAwait(false);

            mreListening.WaitOne();


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
            Console.ReadKey();

            var agent = container.GetInstance<IAgentContext>();
            agent.Stop();

            Thread.Sleep(5000);

            publishProxy.Dispose();
            reqRspProxy.Dispose();
        }
    }
}
