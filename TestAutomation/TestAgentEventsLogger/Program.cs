namespace TestAgentEventsLogger
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using CoenM.ZeroMq.Socket;
    using NLog;
    using SimpleInjector;
    using ZeroMQ;

    public static class Program
    {
        private const string AgentPublishPort = "5556";
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static Container container;
        private static int received;

        public static async Task Main(string[] args)
        {
            container = new Container();
            Bootstrapper.Bootstrap(container);
            container.Verify(VerificationOption.VerifyOnly);

            var mreListening = new ManualResetEvent(false);
            var cts = new CancellationTokenSource();

            Console.WriteLine("Start listening for events");

            var task = Task.Run(
            () =>
            {
                using (var subscriber = container.GetInstance<IZeroMqSocketFactory>().Create(ZSocketType.SUB))
                using (cts.Token.Register(() => subscriber.Dispose()))
                {
                    var endpoint = $"tcp://localhost:{AgentPublishPort}";
                    Logger.Info($"Connect to {endpoint}");
                    subscriber.Connect(endpoint);
                    subscriber.SubscribeAll();

                    try
                    {
                        while (true)
                        {
                            var zmsg = new ZMessage();
                            mreListening.Set();

                            if (!subscriber.ReceiveMessage(ref zmsg, ZSocketFlags.None, out var error))
                            {
                                Logger.Error($" Oops, could not receive a request: {error}");
                                return;
                            }

                            received++;
                            Console.Title = $"Logger - # Events received: {received}";

                            using (zmsg)
                            {
                                Logger.Info(string.Empty);
                                Logger.Info("+" + new string('-', 100 + 2) + "+");
                                bool subscribeUnsubscribe = false;
                                if (zmsg.Count == 1 && zmsg[0].Length == 1)
                                {
                                    var b = zmsg.PopAsByte();
                                    var m = string.Empty;
                                    if (b == 0x01)
                                        m = "SUBSCRIBE  (0x01)";
                                    if (b == 0x00)
                                        m = "UNSUBSCRIBE (0x00)";

                                    if (!string.IsNullOrEmpty(m))
                                    {
                                        subscribeUnsubscribe = true;
                                        Logger.Info($"| {m,-100} |");
                                    }
                                }

                                if (subscribeUnsubscribe == false)
                                {
                                    foreach (var frame in zmsg)
                                    {
                                        var s = frame.ReadString();
                                        Logger.Info($"| {s,-100} |");
                                    }
                                }

                                Logger.Info("+" + new string('-', 100 + 2) + "+");
                                Logger.Info(" ");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (!cts.IsCancellationRequested)
                            Logger.Error(e.Message);
                    }
                }
            },
            CancellationToken.None).ConfigureAwait(false);

            mreListening.WaitOne();

            Console.WriteLine("Done. Press enter to exit.");
            Console.ReadLine();
            cts.Cancel();

            await task;

            Console.WriteLine("Done. Press enter to exit 2.");
            Console.ReadLine();
        }
    }
}
