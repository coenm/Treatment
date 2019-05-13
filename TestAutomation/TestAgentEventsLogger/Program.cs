namespace TestAgentEventsLogger
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using SimpleInjector;
    using TreatmentZeroMq.Socket;
    using ZeroMQ;

    public static class Program
    {
        private const string AgentPublishPort = "5556";

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

            var task = Task.Run(() =>
            {
                using (var subscriber = container.GetInstance<IZeroMqSocketFactory>().Create(ZSocketType.SUB))
                using (cts.Token.Register(() => subscriber.Dispose()))
                {
                    subscriber.Connect($"tcp://localhost:{AgentPublishPort}");
                    subscriber.SubscribeAll();

                    try
                    {
                        while (true)
                        {
                            var zmsg = new ZMessage();
                            ZError error;
                            mreListening.Set();

                            if (!subscriber.ReceiveMessage(ref zmsg, ZSocketFlags.None, out error))
                            {
                                Console.WriteLine($" Oops, could not receive a request: {error}");
                                return;
                            }

                            received++;
                            Console.Title = $"Logger - # Events received: {received}";

                            using (zmsg)
                            {
                                Console.WriteLine();
                                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:fff"));
                                Console.WriteLine("+" + new string('-', 100 + 2) + "+");

                                foreach (var frame in zmsg)
                                {
                                    var s = frame.ReadString();
                                    Console.WriteLine($"| {s,-100} |");
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

            Console.WriteLine("Done. Press enter to exit.");
            Console.ReadLine();
            cts.Cancel();

            await task;

            Console.WriteLine("Done. Press enter to exit 2.");
            Console.ReadLine();
        }
    }
}
