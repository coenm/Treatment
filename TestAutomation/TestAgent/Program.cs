namespace TestAgent
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Medallion.Shell;
    using SimpleInjector;
    using TestAgent.Implementation;
    using TestAgent.ZeroMq.RequestReplyWorker;
    using TreatmentZeroMq.ContextService;
    using ZeroMq.PublishInfrastructure;
    using ZeroMq.RequestReplyInfrastructure;
    using ZeroMQ;

    public static class Program
    {
        private const string AgentReqRspPort = "5555";
        private const string AgentPublishPort = "5556";
        private const string SutPublishPort = "5557";
        private const string SutReqRspPort = "5558";

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
                $"tcp://*:{AgentReqRspPort}",
                $"tcp://*:{AgentPublishPort}");

            container.Verify(VerificationOption.VerifyOnly);

            var context = container.GetInstance<IZeroMqContextService>().GetContext();

            if (FindTreatmentUi(out var treatmentDir, out var executable))
                return;

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
                                zeroMqReqRepProxyFactory.GetConfig().BackendAddress.First(),
                                cts.Token);

            var task = Task.Run(() =>
            {
                int received = 0;
                using (var subscriber = new ZSocket(context, ZSocketType.SUB))
                using (cts.Token.Register(() => subscriber.Dispose()))
                {
                    subscriber.Bind($"tcp://*:{SutPublishPort}");
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

            var command = Command.Run(
                executable,
                new string[0],
                options =>
                {
                    options.WorkingDirectory(treatmentDir);
                    options.CancellationToken(cts.Token);
                    options.EnvironmentVariables(new[]
                    {
                        new KeyValuePair<string, string>("ENABLE_TEST_AUTOMATION", "true"),
                        new KeyValuePair<string, string>("TA_KEY", string.Empty),
                        new KeyValuePair<string, string>("TA_PUBLISH_SOCKET", $"tcp://localhost:{SutPublishPort}"), // sut publishes events on this
                        new KeyValuePair<string, string>("TA_REQ_RSP_SOCKET", $"tcp://*:{SutReqRspPort}"), // sut starts listening for requests on this port.
                    });
                });

            var result = await command.Task.ConfigureAwait(true);

            Console.WriteLine(result.StandardOutput);
            Console.WriteLine(result.StandardError);

            cts.Cancel();
            Console.WriteLine("Done. Press enter to exit.");
            Console.ReadLine();
            await task;

            publishProxy.Dispose();
            reqRspProxy.Dispose();
        }

        private static bool FindTreatmentUi(out string treatmentDir, out string executable)
        {
            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            treatmentDir = currentDir;
            while (!Directory.Exists(Path.Combine(treatmentDir, "src", "Treatment.UI.Start", "bin", "x64", Config)) && treatmentDir?.Length > 4)
            {
                treatmentDir = Path.GetFullPath(Path.Combine(treatmentDir, ".."));
            }

            treatmentDir = Path.GetFullPath(Path.Combine(treatmentDir, "src", "Treatment.UI.Start", "bin", "x64", Config));
            executable = Path.Combine(treatmentDir, "Treatment.UIStart.exe");

            if (!File.Exists(executable))
            {
                Console.WriteLine($"File {executable} doesn't exist.");
                Console.ReadLine();
                return true;
            }

            return false;
        }
    }
}
