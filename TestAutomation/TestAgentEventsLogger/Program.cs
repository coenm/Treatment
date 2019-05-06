namespace TestAgentEventsLogger
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using SimpleInjector;
    using TestAgent.Contract.Interface.Control;
    using TestAgent.Contract.Serializer;
    using TreatmentZeroMq.ContextService;
    using TreatmentZeroMq.Helpers;

    using ZeroMQ;

    public static class Program
    {
        private const string AgentReqRspPort = "5555";
        private const string AgentPublishPort = "5556";

        private static Container container;

        public static async Task Main(string[] args)
        {
            container = new Container();

            Bootstrapper.Bootstrap(container);

            container.Verify(VerificationOption.VerifyOnly);

            var context = container.GetInstance<IZeroMqContextService>().GetContext();

            var mreListening = new ManualResetEvent(false);
            var cts = new CancellationTokenSource();


            using (var socket = new ZSocket(context, ZSocketType.REQ) { Linger = TimeSpan.Zero })
            {
                if (socket.TryConnect($"tcp://localhost:{AgentReqRspPort}"))
                {
                    ZmqConnection.GiveZeroMqTimeToFinishConnectOrBind();

                    var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    var slnDir = currentDir;
                    while (!File.Exists(Path.Combine(slnDir, "Treatment.sln")) && slnDir?.Length > 4)
                    {
                        slnDir = Path.GetFullPath(Path.Combine(slnDir, ".."));
                    }

                    var (type, payload) = RequestResponseSerializer.Serialize(new LocateFilesRequest { Directory = slnDir, Filename = "TestAgent.exe", });

                    var msg = new ZMessage(new List<ZFrame> { new ZFrame(type), new ZFrame(payload), });
                    if (socket.TrySend(msg, 5, i => 10 * i))
                    {
                        if (socket.TryReceive(out var rsp, 5, i => i * 10))
                        {

                            if (rsp.Count >= 2)
                            {
                                var t = rsp.Pop().ReadString();
                                var p = rsp.Pop().ReadString();
                                var resp = RequestResponseSerializer.DeserializeResponse(t, p);
                                if (resp is LocateFilesResponse locateFilesRsp)
                                {
                                    foreach (var e in locateFilesRsp.Executable)
                                    {
                                        Console.WriteLine($"| {e,-100} |");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"| {t,-100} |");
                                    Console.WriteLine($"| {p,-100} |");
                                }
                            }
                            else
                            {
                                foreach (var frame in rsp)
                                {
                                    var s = frame.ReadString();
                                    Console.WriteLine($"| {s,-100} |");
                                }
                            }
                        }
                    }
                }

                socket.Disconnect($"tcp://localhost:{AgentReqRspPort}");
            }

            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
            Console.WriteLine("Start listening for events");

            var task = Task.Run(() =>
            {
                using (var subscriber = new ZSocket(context, ZSocketType.SUB))
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
                                    // read first frame
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

            Console.WriteLine("Done. Press enter to exit.");
            Console.ReadLine();
            cts.Cancel();

            await task;

            Console.WriteLine("Done. Press enter to exit 2.");
            Console.ReadLine();
        }
    }
}
